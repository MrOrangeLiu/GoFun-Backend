using DivingApplication.Entities;
using DivingApplication.Entities.ManyToManyEntities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.ResourceParameters;
using System;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.ChatRooms
{
    public interface IChatRepository
    {
        Task AddChatRoom(ChatRoom chatRoom);
        Task<bool> ChatRoomExists(Guid chatRoomId);
        void DeleteChatRoom(ChatRoom chatRoom);
        Task<ChatRoom> GetChatRoom(Guid chatRoomId);
        Task<PageList<ChatRoom>> GetChatRooms(ChatRoomResourceParameters resourceParameters);
        Task<PageList<ChatRoom>> GetSelfChatRooms(ChatRoomResourceParameters resourceParameters);
        Task<ChatRoom> GetTwoMembersPrivateRoom(Guid userAId, Guid userBId);
        Task<UserChatRoom> GetUserChatRoom(Guid userId, Guid chatRoomId);
        void RemoveUserChatRoom(UserChatRoom userChatRoom);
        Task<bool> Save();
    }
}