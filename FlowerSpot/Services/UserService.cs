using DbExploration.Data;
using FlowerSpot.Models;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;

namespace FlowerSpot.Services
{
    public class UserService : IUserService
    {
        public readonly FlowerDbContext _dbContext;

        public UserService(FlowerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(user => user.Username == username);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var Existinguser = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == user.Username);
            if ( Existinguser == null)
            {
                user.Password = BCryptNet.HashPassword(user.Password);
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
                return user;
            }
            return null;
            
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _dbContext.Users.FindAsync(userId);
        }

        public async Task<User> UpdateUserAsync(User userToUpdate, bool update_pass)
        {
            var existingUser = await _dbContext.Users.FindAsync(userToUpdate.Id);

            if (existingUser == null)
            {
                return null;
            }

            existingUser.Username = userToUpdate.Username;
            if (update_pass)
                existingUser.Password = BCryptNet.HashPassword(userToUpdate.Password);
            existingUser.Email = userToUpdate.Email;

            _dbContext.Users.Update(existingUser);
            await _dbContext.SaveChangesAsync();

            return existingUser;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == username);

            // Check if user exists and password is correct
            if (user == null || !BCryptNet.Verify(password, user.Password))
            {
                return null;
            }

            return user;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            List<User> users = await _dbContext.Users.ToListAsync();
            return users;
        }


    }
}
