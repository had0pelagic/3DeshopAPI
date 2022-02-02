using _3DeshopAPI.Services.Interfaces;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace _3DeshopAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly Context _context;

        public UserService(ILogger<UserService> logger, Context context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Gets users list from db
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();

            _logger.LogInformation(users.ToString());

            return users;
        }

        /// <summary>
        /// Gets user by id from db
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User?> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            _logger.LogInformation(user.ToString());

            return user;
        }
        
        /// <summary>
        /// Adds user by given model to db
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task AddUser(User user)
        {
            _context.Users.Add(user);

            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes user by given model from db
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Task RemoveUser(User user)
        {
            _context.Users.Remove(user);

            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates user by given model to db
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<User?> UpdateUser(User user)
        {
            var dbUser = await _context.Users.FindAsync(user.Id);

            dbUser.FirstName = user.FirstName;
            dbUser.LastName = user.LastName;
            dbUser.Email = user.Email;
            dbUser.Username = user.Username;
            dbUser.Password = user.Password;
            dbUser.ImageURL = user.ImageURL;

            await _context.SaveChangesAsync();

            return dbUser;
        }
    }
}
