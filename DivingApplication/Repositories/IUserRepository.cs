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
        void UpdateUser(User user);
    }
}