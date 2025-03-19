namespace Stride.Data.Models;

public interface IUserRepository
{
    User GetUsers();
    User GetUserById(int id);
    Task<User> GetUserByUsername(string username);
    Task<User> GetUserByEmail(string email);
    User CreateUser(User user);
    User UpdateUser(User user);
    User DeleteUser(int id);
}