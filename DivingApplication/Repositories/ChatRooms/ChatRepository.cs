using DivingApplication.DbContexts;
using DivingApplication.Entities;
using DivingApplication.Entities.ManyToManyEntities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.ChatRoom;
using DivingApplication.Services.PropertyServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.ChatRooms
{
    public class ChatRepository : IChatRepository
    {
        private readonly DivingAPIContext _context;
        private readonly IPropertyMappingService _propertyMapping;

        public ChatRepository(DivingAPIContext context, IPropertyMappingService propertyMapping)
        {
            _context = context;
            _propertyMapping = propertyMapping;
        }


        #region ChatRoom 
        public async Task AddChatRoom(ChatRoom chatRoom)
        {
            if (chatRoom == null) throw new ArgumentNullException(nameof(chatRoom));
            chatRoom.CreatedAt = DateTime.Now;
            await _context.AddRangeAsync(chatRoom);
        }

        public async Task<ChatRoom> GetChatRoom(Guid chatRoomId)
        {
            if (chatRoomId == Guid.Empty) throw new ArgumentNullException(nameof(chatRoomId));

            return await _context.ChatRooms
                .OrderByDescending(c => c.CreatedAt)
                .Include(c => c.Place)
                .Include(c => c.Messages)
                .SingleOrDefaultAsync(c => c.Id == chatRoomId);
        }

        public async Task<UserChatRoom> GetUserChatRoom(Guid userId, Guid chatRoomId)
        {
            return (UserChatRoom)await _context.FindAsync(typeof(UserChatRoom), userId, chatRoomId);
        }

        public async void RemoveUserChatRoom(UserChatRoom userChatRoom)
        {
            _context.Remove(userChatRoom);
        }

        public async Task<PageList<ChatRoom>> GetChatRooms(ChatRoomResourceParameters resourceParameters)
        {
            if (resourceParameters == null) throw new ArgumentNullException(nameof(resourceParameters));

            var collection = _context.ChatRooms
                .Include(c => c.Place)
                .Include(c => c.Messages) //.Take(resourceParameters.NumOfMessages)
                .Include(c => c.UserChatRooms)
                .ThenInclude(ucr => ucr.User) as IQueryable<ChatRoom>;

            if (resourceParameters.MemberId != Guid.Empty)
            {
                // Searching for the ChatRoom that has the member
                collection = collection.Where(c => c.UserChatRooms.Select(uc => uc.UserId).Contains(resourceParameters.MemberId));
            }

            if (!string.IsNullOrWhiteSpace(resourceParameters.SearchQuery))
            {
                collection = collection.Where(c => c.IsGroup == true && c.GroupName.Contains(resourceParameters.SearchQuery));
            }

            if (resourceParameters.Place != null)
            {
                collection = collection.SearchingByPlace(resourceParameters.Place) as IQueryable<ChatRoom>;
            }



            if (!string.IsNullOrWhiteSpace(resourceParameters?.OrderBy?.Replace(",", "")))
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<ChatRoomOutputDto, ChatRoom>();
                collection = collection.ApplySort(resourceParameters.OrderBy, postPropertyMappingDictionary);
            }
            else
            {
                //c.Messages.LastOrDefault().CreatedAt
                collection = collection.OrderByDescending(c => c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault().CreatedAt); // the last // Q: should we do null checking here?
            }


            if (resourceParameters.NumOfMessages != null)
            {
                await collection.ForEachAsync(c => c.Messages = c.Messages.TakeLast(resourceParameters.NumOfMessages).ToList());
            }



            return PageList<ChatRoom>.Create(collection, resourceParameters.PageNumber, resourceParameters.PageSize);
        }

        public async Task<PageList<ChatRoom>> GetSelfChatRooms(ChatRoomResourceParameters resourceParameters)
        {
            if (resourceParameters == null) throw new ArgumentNullException(nameof(resourceParameters));

            User user = await _context.Users
                .Include(u => u.UserChatRooms)
                .ThenInclude(uc => uc.ChatRoom)
                .ThenInclude(c => c.Place)
                .Include(u => u.UserChatRooms)
                .ThenInclude(uc => uc.ChatRoom)
                .ThenInclude(c => c.Messages)
                .Include(u => u.UserChatRooms)
                .ThenInclude(uc => uc.ChatRoom)
                .ThenInclude(c => c.UserChatRooms)
                .ThenInclude(ucr => ucr.User).SingleOrDefaultAsync(u => u.Id == resourceParameters.MemberId);


            var collection = user.UserChatRooms.Select(ucr => ucr.ChatRoom) as IQueryable<ChatRoom>;

            if (!string.IsNullOrWhiteSpace(resourceParameters.SearchQuery))
            {
                collection = collection.Where(c => c.IsGroup == true && c.GroupName.Contains(resourceParameters.SearchQuery));
            }

            if (resourceParameters.Place != null)
            {
                collection = collection.SearchingByPlace(resourceParameters.Place) as IQueryable<ChatRoom>;
            }

            if (!string.IsNullOrWhiteSpace(resourceParameters?.OrderBy?.Replace(",", "")))
            {
                var postPropertyMappingDictionary = _propertyMapping.GetPropertyMapping<ChatRoomOutputDto, ChatRoom>();
                collection = collection.ApplySort(resourceParameters.OrderBy, postPropertyMappingDictionary);
            }
            else
            {
                //c.Messages.LastOrDefault().CreatedAt
                collection = collection.OrderByDescending(c => c.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault().CreatedAt); // the last // Q: should we do null checking here?
            }

            if (resourceParameters.NumOfMessages != null)
            {
                await collection.ForEachAsync(c => c.Messages = c.Messages.TakeLast(resourceParameters.NumOfMessages).ToList());
            }

            return PageList<ChatRoom>.Create(collection, resourceParameters.PageNumber, resourceParameters.PageSize);
        }

        public async Task<bool> ChatRoomExists(Guid chatRoomId)
        {
            if (chatRoomId == Guid.Empty) throw new ArgumentNullException(nameof(chatRoomId));

            return await _context.ChatRooms.AnyAsync(c => c.Id == chatRoomId);
        }

        public async Task<ChatRoom> GetTwoMembersPrivateRoom(Guid userAId, Guid userBId)
        {
            if (userAId == Guid.Empty) throw new ArgumentNullException(nameof(userAId));
            if (userBId == Guid.Empty) throw new ArgumentNullException(nameof(userBId));

            return await _context.ChatRooms
                .Include(c => c.Place)
                .Include(c => c.Messages)
                .Include(c => c.UserChatRooms)
                .ThenInclude(c => c.UserId)
                .SingleOrDefaultAsync(c => c.IsGroup == true
                && c.UserChatRooms.Select(ucr => ucr.UserId).Contains(userAId)
                && c.UserChatRooms.Select(ucr => ucr.UserId).Contains(userBId));
        }


        public void DeleteChatRoom(ChatRoom chatRoom)
        {
            if (chatRoom == null) throw new ArgumentNullException(nameof(chatRoom));

            _context.ChatRooms.Remove(chatRoom);
        }

        public async Task<bool> Save()
        {
            return ((await _context.SaveChangesAsync()) >= 0);
        }

        #endregion


    }
}
