using DivingApplication.Entities;
using System;
using System.Threading.Tasks;

namespace DivingApplication.Repositories
{
    public interface IUserRepository
    {
        Task AddUser(User user, string password);
        User Authenticate(string Email, string password);
        void DeleteUser(User user);
        Task<User> GetUser(Guid userId);
        User GetUserForJwt(Guid userId);
        Task<bool> Save();
        Task UpdateUser(User user);
        Task<bool> UserExists(Guid userId);
    }
}