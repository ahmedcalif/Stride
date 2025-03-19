using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stride.Data.Models;

public class MockUserRepository : IUserRepository
{
    private static readonly List<User> _users;

    static MockUserRepository()
    {
        _users = new List<User>
        {
            new User
            {
                Id = 1,
                Username = "john_doe",
                Email = "john@example.com",
                FirstName = "John",
                LastName = "Doe",
                Password = "password123" 
            },
            new User
            {
                Id = 2,
                Username = "jane_smith",
                Email = "jane@example.com",
                FirstName = "Jane",
                LastName = "Smith",
                Password = "password123"
            },
            new User
            {
                Id = 3,
                Username = "bob_wilson",
                Email = "bob@example.com",
                FirstName = "Bob",
                LastName = "Wilson",
                Password = "password123"
            }
        };
    }

    public User GetUsers()
    {
        return _users.First();
    }

    public User GetUserById(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }
        return user;
    }

    public async Task<User> GetUserByUsername(string username)
{
    // For greater clarity, consider a more explicit comparison:
    var user = _users.FirstOrDefault(u => 
        string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
    return await Task.FromResult(user); 
}

    public async Task<User> GetUserByEmail(string email)
    {
        var user = _users.FirstOrDefault(u => 
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return await Task.FromResult(user);
    }

    public User CreateUser(User user)
    {

        if (_users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
        {
            return null; 
        }

        user.Id = _users.Max(u => u.Id) + 1;
        _users.Add(user);
        return user;
    }

    public User UpdateUser(User user)
    {
        var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Password = user.Password; 
            return existingUser;
        }
        throw new KeyNotFoundException($"User with ID {user.Id} not found");
    }

    public User DeleteUser(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            _users.Remove(user);
            return user;
        }
        throw new KeyNotFoundException($"User with ID {id} not found");
    }
}