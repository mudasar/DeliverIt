using DeliverIt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverIt.Services
{
    public interface IUserService
    {
        Task<User> CreateUser(User model);
        Task<IList<User>> GetAllUsers();
        Task<User> GetUserById(int id);
        Task<User> RemoveUser(int id);
        Task<User> UpdateUser(User user);
        Task<bool> UserExists(int id);
        Task<bool> UserExists(string email);
    }
}
