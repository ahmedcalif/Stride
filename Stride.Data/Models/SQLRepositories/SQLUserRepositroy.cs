using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stride.Data.DatabaseModels;
using Stride.Data.Models;
using Stride.Data;
using Stride.Data.Data;

namespace Stride.Data.Models.SQLRepository
{
    public class SQLUserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SQLUserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Convert DatabaseModels.User to Models.User
        private Models.User ConvertToModelUser(DatabaseModels.User dbUser)
        {
            if (dbUser == null) return null;
            
            return new Models.User
            {
                Id = dbUser.user_id,
                Email = dbUser.email,
                Username = dbUser.username,
                Password = dbUser.password_hash
                // Map other properties as needed
            };
        }

        public Models.User GetUsers()
        {
            // Implementation depends on what this method should return
            // Maybe return the first user or a default user?
            var dbUser = _dbContext.Users.FirstOrDefault();
            return ConvertToModelUser(dbUser);
        }

        public Models.User GetUserById(int id)
        {
            var dbUser = _dbContext.Users.Find(id);
            return ConvertToModelUser(dbUser);
        }

        public async Task<Models.User> GetUserByUsername(string username)
        {
            var dbUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.username == username);
            return ConvertToModelUser(dbUser);
        }

        public async Task<Models.User> GetUserByEmail(string email)
        {
            var dbUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.email == email);
            return ConvertToModelUser(dbUser);
        }

        public Models.User CreateUser(Models.User user)
        {
            // Convert Models.User to DatabaseModels.User
            var dbUser = new DatabaseModels.User
            {
                email = user.Email,
                username = user.Username,
                password_hash = user.Password
                // Set other properties as needed
            };

            _dbContext.Users.Add(dbUser);
            _dbContext.SaveChanges();
            
            // Return the created user with ID
            return ConvertToModelUser(dbUser);
        }

        public Models.User UpdateUser(Models.User user)
        {
            var dbUser = _dbContext.Users.Find(user.Id);
            if (dbUser == null)
            {
                return null;
            }

            // Update properties
            dbUser.email = user.Email;
            dbUser.username = user.Username;
            dbUser.password_hash = user.Password;
            // Update other properties as needed
            
            _dbContext.Users.Update(dbUser);
            _dbContext.SaveChanges();
            
            return ConvertToModelUser(dbUser);
        }

        public Models.User DeleteUser(int id)
        {
            var dbUser = _dbContext.Users.Find(id);
            if (dbUser == null)
            {
                return null;
            }
            
            _dbContext.Users.Remove(dbUser);
            _dbContext.SaveChanges();
            
            return ConvertToModelUser(dbUser);
        }
    }
}