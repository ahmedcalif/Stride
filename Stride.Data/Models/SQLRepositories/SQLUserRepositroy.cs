using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stride.Data.Models;
using Stride.Data.Data;
using Stride.Data.DatabaseModels;

namespace Stride.Data.Models.SQLRepository
{
    public class SQLUserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;

        public SQLUserRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public User GetUsers()
        {
            var dbUser = _context.Users.FirstOrDefault();
            if (dbUser == null) return null;
            
            return MapDbUserToModelUser(dbUser);
        }

        public User GetUserById(int id)
        {
            var dbUser = _context.Users.Find(id);
            if (dbUser == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }
            
            return MapDbUserToModelUser(dbUser);
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var dbUser = await _context.Users
                .FirstOrDefaultAsync(u => u.username.ToLower() == username.ToLower());
            
            return dbUser != null ? MapDbUserToModelUser(dbUser) : null;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var dbUser = await _context.Users
                .FirstOrDefaultAsync(u => u.email.ToLower() == email.ToLower());
            
            return dbUser != null ? MapDbUserToModelUser(dbUser) : null;
        }

        public User CreateUser(User user)
        {
            
            var dbUser = new DatabaseModels.User
            {
                email = user.Email,
                password_hash = user.Password,
                username = user.Username,
            };
             if (string.IsNullOrEmpty(user.Password))
            {
        user.Password = "IDENTITY_MANAGED"; 
             }

            _context.Users.Add(dbUser);
            _context.SaveChanges();
            
            user.Id = dbUser.user_id;
            return user;
        }

        public User UpdateUser(User user)
        {
            var dbUser = _context.Users.Find(user.Id);
            if (dbUser == null)
                throw new KeyNotFoundException($"User with ID {user.Id} not found");

            dbUser.email = user.Email;
            dbUser.username = user.Username;
            
            _context.SaveChanges();
            return user;
        }

        public User DeleteUser(int id)
        {
            var dbUser = _context.Users.Find(id);
            if (dbUser == null)
                throw new KeyNotFoundException($"User with ID {id} not found");
            
            var user = MapDbUserToModelUser(dbUser);
            _context.Users.Remove(dbUser);
            _context.SaveChanges();
            
            return user;
        }
        
        private User MapDbUserToModelUser(DatabaseModels.User dbUser)
        {
            return new User
            {
                Id = dbUser.user_id,
                Email = dbUser.email,
                Password = dbUser.password_hash,
                Username = dbUser.username
            };
        }
    }
}