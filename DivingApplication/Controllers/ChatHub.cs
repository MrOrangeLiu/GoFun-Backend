using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Entities.ManyToManyEntities;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Models.ChatRooms;
using DivingApplication.Models.Messages;
using DivingApplication.Models.Users;
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
using static DivingApplication.Entities.ManyToManyEntities.UserChatRoom;
using static DivingApplication.Entities.Message;

namespace DivingApplication.Controllers
{
    [Authorize(Policy = "ExceptBanned")]
    public class ChatHub : Hub
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        static public Dictionary<Guid, String> UserIdToConnectionIdDict;



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

            // Connect to all the ChatRoom
            //  Where(ucr => ucr.Role != UserChatRoomRole.Pending).
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

        //public async Task<ChatRoomOutputDto> CreatePrivateChatRoom(string anotherUserIdString)
        //{

        //    var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
        //    var anotherUserId = Guid.Parse(anotherUserIdString);

        //    // Create ChatRoom

        //    var newChatRoom = new ChatRoom()
        //    {
        //        IsGroup = false,
        //    };

        //    await _chatRepository.AddChatRoom(newChatRoom);

        //    // Add two members in 

        //    newChatRoom.UserChatRooms.AddRange(
        //        new List<UserChatRoom> {
        //            new UserChatRoom()
        //                {
        //                    ChatRoomId = newChatRoom.Id,
        //                    UserId = userId,
        //                },
        //            new UserChatRoom()
        //                {
        //                    ChatRoomId = newChatRoom.Id,
        //                    UserId = anotherUserId,
        //                },
        //        }
        //    );

        //    await _chatRepository.Save();

        //    var chatRoomToReturn = _mapper.Map<ChatRoomOutputDto>(newChatRoom);

        //    if (UserIdToConnectionIdDict.ContainsKey(anotherUserId))
        //    {
        //        // Telling another user
        //        await NotifyNewChatRoom(UserIdToConnectionIdDict[anotherUserId], chatRoomToReturn);
        //    }

        //    return chatRoomToReturn;
        //}

        ////ChatRoomForCreatingDto chatRoom
        //public async Task<ChatRoomOutputDto> CreateGroupChatRoom(ChatRoomForCreatingDto chatRoom)
        //{
        //    // Get CurrentUser Id
        //    var userId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));

        //    // Transform to Entity
        //    var chatRoomEntity = _mapper.Map<ChatRoom>(chatRoom);

        //    // Adding this ChatRoom to Db for Id
        //    await _chatRepository.AddChatRoom(chatRoomEntity);

        //    // Modify the Required Fields
        //    chatRoomEntity.IsGroup = true;
        //    chatRoomEntity.CreatedAt = DateTime.Now;
        //    chatRoomEntity.UserChatRooms.Add(
        //            new UserChatRoom()
        //            {
        //                ChatRoomId = chatRoomEntity.Id,
        //                UserId = userId,
        //                Role = UserChatRoom.UserChatRoomRole.Owner,
        //            }
        //        );

        //    // Save to Db
        //    await _chatRepository.Save();

        //    // Tranform to ReturnType
        //    var chatRoomToReturn = _mapper.Map<ChatRoomOutputDto>(chatRoomEntity);

        //    // Return the ChatRoom
        //    return chatRoomToReturn;
        //}

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
            if (user.UserRole != User.Role.Admin && userChatRoom.Role != UserChatRoom.UserChatRoomRole.Owner && userChatRoom.Role != UserChatRoom.UserChatRoomRole.Admin) throw new Exception("Not Authorised");

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
            await Clients.Group(chatRoomIdString).SendAsync("OnChatRoomChanged", chatRoomToReturn); //TODO: Complete this in Client Side



            return chatRoomToReturn;
        }

        public async Task AddUserToGroupChatRoom(Guid userId, Guid roomId)
        {
            // Check the user exists
            var invitedUser = await _userRepository.GetUser(userId);
            if (invitedUser == null) throw new Exception("Cannot find the user for invitation");

            // Checking the ChatRoom exists
            var chatRoomToJoin = await _chatRepository.GetChatRoom(roomId);
            if (chatRoomToJoin == null) throw new Exception("Cannot find the ChatRoom to join");

            // Checking the Connection doesn't exist
            var currentConnection = await _chatRepository.GetUserChatRoom(userId, roomId);
            if (currentConnection != null) throw new Exception("User already in this ChatRoom");


            // Checking the LoginUser has the right to Adding members
            var loginUserId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var loginUser = await _userRepository.GetUser(loginUserId);
            if (loginUser.UserRole != User.Role.Admin)
            {
                var loginUserChatRoom = await _chatRepository.GetUserChatRoom(loginUserId, roomId);
                if (loginUserChatRoom.Role != UserChatRoom.UserChatRoomRole.Owner && loginUserChatRoom.Role != UserChatRoom.UserChatRoomRole.Admin)
                {
                    throw new Exception("User not allowed to invite new members");
                }
            }

            // Creating the Connection
            var connection = new UserChatRoom()
            {
                UserId = invitedUser.Id,
                ChatRoomId = chatRoomToJoin.Id,
                Role = UserChatRoom.UserChatRoomRole.Pending, // Waiting for accepting the invitation.
            };

            // Adding the Connection
            chatRoomToJoin.UserChatRooms.Add(connection);

            // Save to Db
            await _chatRepository.Save();


            // Invoke the Client listener 
            var userToReturn = _mapper.Map<UserBriefOutputDto>(invitedUser);

            // adding the Connection to this user, Connect Before Join
            if (UserIdToConnectionIdDict.ContainsKey(invitedUser.Id))
            {
                await Groups.AddToGroupAsync(UserIdToConnectionIdDict[invitedUser.Id], chatRoomToJoin.Id.ToString());
            }

            await Clients.Group(chatRoomToJoin.Id.ToString()).SendAsync("OnNewMemberJoin", chatRoomToJoin.Id, userToReturn);


            await SendMessage(new MessageForCreatingDto()
            {
                BelongChatRoomId = chatRoomToJoin.Id,
                Content = $"{loginUser.Name} 邀請了 {invitedUser.Name}",
                MessageType = MessageContentType.System.ToString(),
            }
            );
        }

        /// <summary>
        /// Remove the User From a Group ChatRoom, the Loggin user has to higher right to remove another user.
        /// </summary>
        /// <param name="userId">
        /// The userId that will be removed
        /// </param>
        /// <param name="roomId">
        /// The ChattRoom that user current stay in
        /// </param>
        /// <returns>
        /// Nothing will return, but the OnChatRoomChanged will be called in the Client
        /// </returns>
        /// 
        public async Task RemoveMemberFromChatRoom(Guid userId, Guid roomId)
        {
            // Check the user exists
            var userToRemove = await _userRepository.GetUser(userId);
            if (userToRemove == null) throw new Exception("Cannot find the user to remove");

            // Checking if the removeUser has the Connection to the ChatRoom
            var removeUserChatRoom = await _chatRepository.GetUserChatRoom(userToRemove.Id, roomId);
            if (removeUserChatRoom == null) throw new Exception("User not in this ChatRoom");

            // Checking the ChatRoom exists 
            var chatRoom = await _chatRepository.GetChatRoom(roomId);
            if (chatRoom == null) throw new Exception("Cannot find the ChatRoom");

            // Checking the LoginUser has the right to remove this member
            var loginUserId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var loginUser = await _userRepository.GetUser(loginUserId);
            if (loginUser.UserRole != User.Role.Admin)
            {
                // Checking if the loginUser want to remove itself.
                if (loginUserId != userId)
                {

                    if (chatRoom.IsGroup != true) throw new Exception("Removing another user in Private ChatRoom is not allowed");
                    // Checking if the loginUser has the right to remove this User
                    var loginUserChatRoom = await _chatRepository.GetUserChatRoom(loginUserId, roomId);
                    if (loginUserChatRoom.Role.HasHigherRole(removeUserChatRoom.Role) <= 0) throw new Exception("User not allowed to remove the member");

                    // Checking if the target user is Onwer
                    if (removeUserChatRoom.Role == UserChatRoomRole.Owner) throw new Exception("Owner is not allowed to be remove");

                }
                else
                {

                    // Checking if the User is the Last one
                    if (chatRoom.UserChatRooms.Count > 1)
                    {
                        // If the user is not the last one, do owner checking.

                        // Checking if the user is Owner
                        if (removeUserChatRoom.Role == UserChatRoomRole.Owner)
                        {
                            // Checking if the chatRoom only has one user now (LoginUser) // Owner
                            if (chatRoom.UserChatRooms.Count != 1) throw new Exception("Owner can't leave the chatRoom before assigning next owner");
                        }
                    }

                }

            }

            // Remove the Connection
            _chatRepository.RemoveUserChatRoom(removeUserChatRoom);

            // Save to Db
            await _chatRepository.Save();


            // Checking how many members in the ChatRoom, if is 0 => Delete the ChatRoom
            if (chatRoom.UserChatRooms.Count <= 0)
            {
                // Remove the Chat Room
                _chatRepository.RemoveChatRoom(chatRoom);
                await _chatRepository.Save();
            }
            else
            {
                string systemMessageContent;
                if (userToRemove.Id == loginUser.Id)
                {
                    systemMessageContent = $"{loginUser.Name} 離開了群組";
                }
                else
                {
                    systemMessageContent = $"{loginUser.Name} 踢除了 {userToRemove.Name}";
                }


                // Sending System Message to ChatRoom
                await SendMessage(new MessageForCreatingDto()
                {
                    BelongChatRoomId = roomId,
                    Content = systemMessageContent,
                    MessageType = MessageContentType.System.ToString(),
                });
            }

            // Changing the ChatRoom in Clients
            await Clients.Group(chatRoom.Id.ToString()).SendAsync("OnMemberRemoved", chatRoom.Id, userToRemove.Id);


            // Then remove the member connection from the Group
            if (UserIdToConnectionIdDict.ContainsKey(userToRemove.Id))
            {
                Groups.RemoveFromGroupAsync(UserIdToConnectionIdDict[userToRemove.Id], chatRoom.Id.ToString());
            }

        }


        public async Task ChangeGroupChatRoomMemberRole(Guid userId, Guid roomId, string role)
        {
            List<UserChatRoomRole> allowedChangedRole = new List<UserChatRoomRole>() {
                UserChatRoomRole.Admin,
                UserChatRoomRole.Owner,
                UserChatRoomRole.Normal,
            };

            var changeToRole = (UserChatRoomRole)Enum.Parse(typeof(UserChatRoomRole), role);

            // Checking if the destination role is allowed
            if (!allowedChangedRole.Contains(changeToRole)) throw new Exception("Not allowed to change to this Role");

            // Check the user exists
            var userToChangeRole = await _userRepository.GetUser(userId);
            if (userToChangeRole == null) throw new Exception("Cannot find the user to remove");

            // Checking if the user has the Connection to the ChatRoom
            var RoleChangedUserChatRoom = await _chatRepository.GetUserChatRoom(userToChangeRole.Id, roomId);
            if (RoleChangedUserChatRoom == null) throw new Exception("User not in this ChatRoom");

            // Checking the destination role is not the same as current role
            if (RoleChangedUserChatRoom.Role == changeToRole) throw new Exception($"The user already own this role: {changeToRole.ToString()}");

            // Checking the LoginUser has the right to change anotherUser Role
            var loginUserId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var loginUser = await _userRepository.GetUser(loginUserId);
            if (loginUser == null) throw new Exception("Can't find this user");

            if (loginUser.UserRole != User.Role.Admin)
            {
                var loginUserChatRoom = await _chatRepository.GetUserChatRoom(loginUserId, roomId);

                // Only owner can change the role
                if (loginUserChatRoom.Role != UserChatRoomRole.Owner) throw new Exception("Only Owner can change others Role");

                //if (loginUserChatRoom.Role.HasHigherRole(RoleChangedUserChatRoom.Role) <= 0) throw new Exception("User not allowed to invite new members");
            }


            UserChatRoom originalOwnerUserChatRoom;

            // Checking if it's a owner re-assignment
            if (changeToRole == UserChatRoomRole.Owner)
            {
                // The original Owner will be downgraded to Normal, since no 2nd Owner allowed
                var chatRoom = await _chatRepository.GetChatRoom(roomId);
                if (chatRoom == null) throw new Exception("Cannot find the ChatRoom");

                originalOwnerUserChatRoom = chatRoom.UserChatRooms.SingleOrDefault(ucr => ucr.Role == UserChatRoomRole.Owner);
                if (originalOwnerUserChatRoom == null) throw new Exception("We have no owner or more than two owners in this ChatRoom");

                originalOwnerUserChatRoom.Role = UserChatRoomRole.Normal;
            }

            // Change the Role
            var previousRole = RoleChangedUserChatRoom.Role;
            RoleChangedUserChatRoom.Role = changeToRole;

            // Save to Db
            await _chatRepository.Save();

            // Checking the ChatRoom exists (Checking here since we want to retrieve the changed data)
            //var chatRoomEntity = await _chatRepository.GetChatRoom(roomId);
            //if (chatRoomEntity == null) throw new Exception("Cannot find the ChatRoom");

            // Changing the ChatRoom in Clients
            await Clients.Group(roomId.ToString()).SendAsync("OnChatRoomUserRoleChanged", roomId, userToChangeRole.Id, changeToRole.ToString());

            // If the user was having the Pending Role, send a Join Notification

            string systemMessageContent;

            switch (changeToRole)
            {
                case UserChatRoomRole.Normal:
                    systemMessageContent = $"{userToChangeRole.Name} 成為了會員";
                    break;
                case UserChatRoomRole.Admin:
                    systemMessageContent = $"{userToChangeRole.Name} 成為了管理員";
                    break;
                case UserChatRoomRole.Owner:
                    systemMessageContent = $"{userToChangeRole.Name} 成為了組長";
                    break;
                default:
                    throw new Exception("Not supporting Role");
            }

            await SendMessage(new MessageForCreatingDto()
            {
                BelongChatRoomId = roomId,
                Content = systemMessageContent,
                MessageType = MessageContentType.System.ToString(),
            }
            );


        }


        public async Task UserAcceptChatRoomInvitation(Guid roomId)
        {

            // Checking the LoginUser Exist
            var loginUserId = Guid.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var loginUser = await _userRepository.GetUser(loginUserId);
            if (loginUser == null) throw new Exception("Can't find this user");



            // Checking if the user has the Connection to the ChatRoom
            var userChatRoom = await _chatRepository.GetUserChatRoom(loginUserId, roomId);
            if (userChatRoom == null) throw new Exception("User not in this ChatRoom");

            // Checking if the user has the Pending Role
            if (userChatRoom.Role != UserChatRoomRole.Pending) throw new Exception($"The user is not pending, but: {userChatRoom.Role.ToString()}");


            userChatRoom.Role = UserChatRoomRole.Normal;

            await _chatRepository.Save();

            // Adding User to the Group
            //if (UserIdToConnectionIdDict.ContainsKey(loginUser.Id))
            //{
            //    Groups.AddToGroupAsync(UserIdToConnectionIdDict[loginUser.Id], roomId.ToString());
            //}

            // Notify Clients
            await Clients.Group(roomId.ToString()).SendAsync("OnChatRoomUserRoleChanged", roomId, loginUser.Id, UserChatRoomRole.Normal.ToString());


            // Sending System Message to ChatRoom
            await SendMessage(new MessageForCreatingDto()
            {
                BelongChatRoomId = roomId,
                Content = $"{loginUser.Name} 加入了",
                MessageType = MessageContentType.System.ToString(),
            }
            );

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

            messageEntity.CreatedAt = DateTime.Now;

            messageEntity.AuthorId = userId;

            // Checking if the ChatRoom Exist

            var chatRoom = await _chatRepository.GetChatRoom(message.BelongChatRoomId);
            if (chatRoom == null) throw new Exception("Can't find this ChatRoom");

            // Get all the pending Users






            if (!(await _chatRepository.ChatRoomExists(message.BelongChatRoomId))) throw new Exception("Cannot find this ChatRoom");

            await _messageRepository.AddMessage(messageEntity);

            await _messageRepository.Save();

            var messageToReturn = _mapper.Map<MessageOutputDto>(messageEntity);


            // Find the Pending Users, not sending the Message to Them
            var pendingConnections = chatRoom.UserChatRooms.Where(ucr => ucr.Role == UserChatRoomRole.Pending && UserIdToConnectionIdDict.ContainsKey(ucr.UserId)).Select(ucr => UserIdToConnectionIdDict[ucr.UserId]).ToList();

            await Clients.GroupExcept(messageToReturn.BelongChatRoomId.ToString(), pendingConnections).SendAsync("OnMessage", messageToReturn);

            // Sending to the Group
            //  await Clients.Group(messageToReturn.BelongChatRoomId.ToString()).SendAsync("OnMessage", messageToReturn);


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
