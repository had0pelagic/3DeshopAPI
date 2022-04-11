using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Models.User;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _3DeshopAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly Context _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBalanceService _balanceService;

        public UserService(ILogger<UserService> logger, IMapper mapper, IHttpContextAccessor contextAccessor, Context context, IBalanceService balanceService)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
            _contextAccessor = contextAccessor;
            _balanceService = balanceService;
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

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            return user;
        }

        /// <summary>
        /// Returns UserDisplayModel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<UserDisplayModel> GetDisplayUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            return _mapper.Map<UserDisplayModel>(user);
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
            await _balanceService.BalanceTopUp(model.Id, 0);
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
            var dbUser = await _context
                .Users
                .FirstOrDefaultAsync(x => EF.Functions.Collate(x.Username, "SQL_Latin1_General_CP1_CS_AS") == model.Username);

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
        /// Returns all user purchased product ids
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<Guid>> GetPurchasedIds(Guid id)
        {
            return await _balanceService.GetPurchasedIds(id);
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
