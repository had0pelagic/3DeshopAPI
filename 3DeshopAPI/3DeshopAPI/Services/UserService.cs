using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Models.User;
using _3DeshopAPI.Services.Interfaces;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _3DeshopAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly Context _context;

        public UserService(ILogger<UserService> logger, IHttpContextAccessor contextAccessor, Context context)
        {
            _logger = logger;
            _context = context;
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Gets users list from db
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();

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

            return user;
        }

        /// <summary>
        /// Adds user by given model to db
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task RegisterUser(User model)
        {
            var userExists = UsernameExists(model.Username);

            if (userExists)
            {
                throw new InvalidClientOperationException(ErrorCodes.InvalidUsername);
            }

            model.UserRole = UserRoles.User;
            model.ImageURL = "https://images.random/defaultimage.jp";

            _context.Users.Add(model);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes user by given model from db
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task RemoveUser(User model)
        {
            _context.Users.Remove(model);

            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates user by given model to db
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateUser(Guid id, User model)
        {
            if (!UserExists(id))
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var dbUser = await _context.Users.FindAsync(id);

            if (model.FirstName != null)
            {
                dbUser.FirstName = model.FirstName;
            }
            if (model.LastName != null)
            {
                dbUser.LastName = model.LastName;
            }
            if (model.Email != null)
            {
                dbUser.Email = model.Email;
            }
            if (model.Username != null)
            {
                dbUser.Username = model.Username;
            }
            if (model.ImageURL != null)
            {
                dbUser.ImageURL = model.ImageURL;
            }

            _context.Entry(dbUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        /// <summary>
        /// Changes user password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> ChangePassword(Guid id, UserPasswordModel model)
        {
            if (!UserExists(id))
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var dbUser = await _context.Users.FindAsync(id);

            if (model.Password != model.ConfirmPassword)
            {
                throw new InvalidClientOperationException(ErrorCodes.BadPassword);
            }
            if (dbUser.Password == model.Password)
            {
                throw new InvalidClientOperationException(ErrorCodes.SamePassword);
            }

            dbUser.Password = model.Password;

            _context.Entry(dbUser).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        /// <summary>
        /// Checks if username and users password is valid
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<User?> IsUserValid(UserLoginModel model)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == model.Username);

            if (dbUser == null || dbUser?.Password != model.Password)
            {
                return null;
            }

            return dbUser;
        }

        /// <summary>
        /// Checks for users role
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<string?> GetUserRole(UserLoginModel model)
        {
            var user = await _context.Users.Where(x => x.Username == model.Username).FirstAsync();

            return user.UserRole;
        }

        /// <summary>
        /// Gets username by id
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<string?> GetUsername(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            return user.Username;
        }

        /// <summary>
        /// Returns user details
        /// </summary>
        /// <returns></returns>
        public User GetCurrentUser()
        {
            var username = _contextAccessor.HttpContext.User.Identity.Name;

            return _context.Users.FirstOrDefault(x => x.Username == username);
        }

        /// <summary>
        /// Checks if username exists
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private bool UsernameExists(string username)
        {
            return _context.Users.Any(x => x.Username == username);
        }

        /// <summary>
        /// Checks if user exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool UserExists(Guid id)
        {
            return _context.Users.Any(x => x.Id == id);
        }
    }
}
