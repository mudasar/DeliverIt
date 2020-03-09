using DeliverIt.Data;
using DeliverIt.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverIt.Services
{
    public class UserService : IUserService
    {

        private readonly DeliverItContext db;
        public UserService(DeliverItContext context)
        {
            this.db = context;
        }

        public async Task<bool> UserExists(int id)
        {
            return await db.Users.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> UserExists(string email)
        {
            return await db.Users.AnyAsync(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IList<User>> GetAllUsers()
        {
            return await db.Users.ToListAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await db.Users.FirstAsync(x => x.Id == id);
        }

        public async Task<User> CreateUser(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return user;
        }

        public async Task<User> RemoveUser(int id)
        {
            var user = await this.GetUserById(id);
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return user;
        }
    }
}
