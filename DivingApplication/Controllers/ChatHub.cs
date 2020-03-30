using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Entities.ManyToManyEntities;
using DivingApplication.Models.ChatRoom;
using DivingApplication.Models.Messages;
using DivingApplication.Repositories.ChatRooms;
using DivingApplication.Repositories.Messages;
using DivingApplication.Repositories.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DivingApplication.Controllers
{
    [Authorize(Policy = "VerifiedUsers")]
    public class ChatHub : Hub
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private Dictionary<Guid, String> UserIdToConnectionIdDict;



        public ChatHub(IUserRepository userRepository, IChatRepository chatRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _chatRepository = chatRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
            UserIdToConnectionIdDict = new Dictionary<Guid, string>();
        }


        public override async Task OnConnectedAsync()
        {
            var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var user = await _userRepository.GetUser(userId);

            if (user == null) throw new Exception("Can't find this user in the Database");

            List<Guid> chatRoomIds = user.UserChatRooms.Select(c => c.ChatRoomId).ToList();

            if (chatRoomIds != null && chatRoomIds.Count != 0)
            {
                foreach (var chatRoomId in chatRoomIds)
                {
                    if (chatRoomId != null)
                        await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId.ToString());
                }
            }


            UserIdToConnectionIdDict.Add(userId, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            UserIdToConnectionIdDict.Remove(userId);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task<ChatRoomOutputDto> CreatePrivateChatRoom(string anotherUserIdString)
        {

            var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var anotherUserId = Guid.Parse(anotherUserIdString);

            // Create ChatRoom

            var newChatRoom = new ChatRoom()
            {
                IsGroup = false,
            };

            await _chatRepository.AddChatRoom(newChatRoom);

            // Add two members in 

            newChatRoom.UserChatRooms.AddRange(
                new List<UserChatRoom> {
                    new UserChatRoom()
                        {
                            ChatRoomId = newChatRoom.Id,
                            UserId = userId,
                        },
                    new UserChatRoom()
                        {
                            ChatRoomId = newChatRoom.Id,
                            UserId = anotherUserId,
                        },
                }
            );

            await _chatRepository.Save();

            var chatRoomToReturn = _mapper.Map<ChatRoomOutputDto>(newChatRoom);

            if (UserIdToConnectionIdDict.ContainsKey(anotherUserId))
            {
                // Telling another user
                await NotifyNewChatRoom(UserIdToConnectionIdDict[anotherUserId], chatRoomToReturn);
            }

            return chatRoomToReturn;
        }


        public async Task<ChatRoomOutputDto> CreateGroupChatRoom(ChatRoomForCreatingDto chatRoom)
        {

            // Get CurrentUser Id
            var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Transform to Entity
            var chatRoomEntity = _mapper.Map<ChatRoom>(chatRoom);

            // Adding this ChatRoom to Db for Id
            await _chatRepository.AddChatRoom(chatRoomEntity);

            // Modify the Required Fields
            chatRoomEntity.IsGroup = true;
            chatRoomEntity.CreatedAt = DateTime.Now;
            chatRoomEntity.UserChatRooms.Add(
                    new UserChatRoom()
                    {
                        ChatRoomId = chatRoomEntity.Id,
                        UserId = userId,
                        Role = UserChatRoom.UserChatRoomRole.Ownder,
                    }
                );

            // Save to Db
            await _chatRepository.Save();

            // Tranform to ReturnType
            var chatRoomToReturn = _mapper.Map<ChatRoomOutputDto>(chatRoomEntity);

            // Return the ChatRoom
            return chatRoomToReturn;
        }

        public async Task<ChatRoomOutputDto> UpdateGroupChatRoom(string chatRoomIdString, JsonPatchDocument<ChatRoomUpdatingDto> patchDocument)
        {
            if (string.IsNullOrWhiteSpace(chatRoomIdString)) throw new Exception("chatRoomIdString is needed");

            // Get User
            var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userRepository.GetUser(userId);

            // Get ChatRoom
            var chatRoomId = Guid.Parse(chatRoomIdString);
            var chatRoomFromRepo = await _chatRepository.GetChatRoom(chatRoomId);

            // Get userChatRoom

            var userChatRoom = await _chatRepository.GetUserChatRoom(userId, chatRoomId);

            // Checking if the ChatRoom Exists
            if (chatRoomFromRepo == null) throw new Exception("Cannot find ChatRoom");

            // Checking if the user have the right to edite the chatRoom, by 3 Conditions:
            // 1. Is the owner of the ChatRoom
            // 2. Is the Admin in this App
            // 3. Is the Admin in this ChatRoom
            if (user.UserRole != User.Role.Admin && userChatRoom.Role != UserChatRoom.UserChatRoomRole.Ownder && userChatRoom.Role != UserChatRoom.UserChatRoomRole.Admin) throw new Exception("Not Authorised");

            // Transform the Entity to UpdatingDto (Prepare for Patching)
            var chatRoomToPatch = _mapper.Map<ChatRoomUpdatingDto>(chatRoomFromRepo);

            // Apply the Patch to UpdatingDto (From Repo)
            patchDocument.ApplyTo(chatRoomToPatch);

            // Validate the Model
            if (!chatRoomToPatch.IsValidModel()) throw new Exception("The updating value is not valid");

            // Using the UpdatingDto to Override the ChatRoomFromRepo
            _mapper.Map(chatRoomToPatch, chatRoomFromRepo); // Overriding

            // Saving to Db
            await _chatRepository.Save();

            var chatRoomToReturn = _mapper.Map<ChatRoomOutputDto>(chatRoomFromRepo);

            // Notify All Users that the ChatRoom has been Updated
            Clients.Group(chatRoomIdString).SendAsync("OnChatRoomChanged", chatRoomToReturn); //TODO: Complete this in Client Side

            return chatRoomToReturn;
        }


        public async Task<ChatRoomOutputDto> AddToRoom(string chatRoomString)
        {
            if (string.IsNullOrWhiteSpace(chatRoomString)) throw new ArgumentNullException(nameof(chatRoomString));

            var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var chatRoomId = Guid.Parse(chatRoomString);

            // Insert to the ChatRoom

            var ChatRoom = await _chatRepository.GetChatRoom(chatRoomId);

            ChatRoom.UserChatRooms.Add(
                new Entities.ManyToManyEntities.UserChatRoom()
                {
                    ChatRoomId = ChatRoom.Id,
                    UserId = userId,
                }
                );

            var ok = await _chatRepository.Save();

            if (ok)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomString);
            }


            return _mapper.Map<ChatRoomOutputDto>(ChatRoom);
        }

        public async Task RemoveUserFromRoom(string chatRoomString)
        {
            if (string.IsNullOrWhiteSpace(chatRoomString)) throw new ArgumentNullException(nameof(chatRoomString));

            var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var chatRoomId = Guid.Parse(chatRoomString);

            var userChatRoom = await _chatRepository.GetUserChatRoom(userId, chatRoomId);

            if (userChatRoom == null) throw new Exception("User doesn't join this ChatRoom");

            _chatRepository.RemoveUserChatRoom(userChatRoom);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomString);
        }

        public void Send(string name, string message)
        {
            Clients.All.SendAsync("OnMessage", new object[] { name, message });
        }

        public async Task NotifyNewChatRoom(string connectionId, ChatRoomOutputDto chatRoom)
        {
            await Clients.Client(connectionId).SendAsync("NewChatRoomCreated", chatRoom);
        }

        public void SendMessageTest(Message message)
        {
            Clients.All.SendAsync("OnMessageObject", new object[] { message });
        }

        public async Task<MessageOutputDto> SendMessage(MessageForCreatingDto message)
        {
            var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var messageEntity = _mapper.Map<Message>(message);

            messageEntity.AuthorId = userId;

            // Checking if the ChatRoom Exist

            if (!(await _chatRepository.ChatRoomExists(message.BelongChatRoomId))) throw new Exception("Cannot find this ChatRoom");

            await _messageRepository.AddMessage(messageEntity);

            await _messageRepository.Save();

            var messageToReturn = _mapper.Map<MessageOutputDto>(messageEntity);

            // Sending to the Group
            Clients.Group(messageToReturn.Id.ToString()).SendAsync("OnMessage", messageToReturn);

            return messageToReturn;
        }


        //[Authorize(Policy = "VerifiedUsers")]
        public MessageOutputDto MethodOneSimpleParameterSimpleReturnValue(Message message)
        {
            Console.WriteLine($"'MethodOneSimpleParameterSimpleReturnValue' invoked. Parameter value: '{message.Content}");
            var messageToReturn = _mapper.Map<MessageOutputDto>(message);

            Clients.All.SendAsync("OnMessageObject", messageToReturn);

            return messageToReturn;
        }
    }
}
