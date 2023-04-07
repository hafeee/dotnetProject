using FlowerSpot.Models;

namespace FlowerSpot.Services
{
    public interface IUserService
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> CreateUserAsync(User user);
        Task<User> GetUserByIdAsync(int userId);

        Task<User> UpdateUserAsync(User userToUpdate, bool updatePass);
        Task<User> AuthenticateAsync(string username, string password);
        Task<List<User>> GetAllUsersAsync();

    }
}
