using DivingApplication.Entities;
using DivingApplication.Entities.ManyToManyEntities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DivingApplication.Repositories.Users
{
    public interface IUserRepository
    {
        Task AddUser(User user, string password);
        User Authenticate(string Email, string password);
        void DeleteUser(User user);
        Task<IEnumerable<User>> GetAllFollowers(Guid userId);
        Task<IEnumerable<User>> GetAllFollowing(Guid userId);
        Task<IEnumerable<Post>> GetAllLikePosts(Guid userId);
        Task<IEnumerable<Post>> GetAllOwningPost(Guid userId);
        Task<IEnumerable<Post>> GetAllSavePosts(Guid userId);
        Task<CoachInfo> GetCoachInfoForUser(Guid userId);
        Task<UserFollow> GetCurrentUserFollow(Guid followerId, Guid followingId);
        Task<IEnumerable<ServiceInfo>> GetServiceInfoForUser(Guid userId);
        Task<List<ServiceInfo>> GetServiceInfosForUser(Guid userId);
        Task<User> GetUser(Guid userId);
        User GetUserForJwt(Guid userId);
        PageList<User> GetUsers(UserResourceParameterts userResourceParameters, Guid loginUserId);
        Task<bool> Save();
        Task UpdateUser(User user);
        Task<bool> UserEmailExists(string email);
        Task<bool> UserExists(Guid userId);
        Task<UserFollow> UserFollowUser(Guid followerId, Guid followingId);
        void UserUnFollowUser(UserFollow currentUserFollow);
    }
}