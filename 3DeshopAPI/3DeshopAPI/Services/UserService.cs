using _3DeshopAPI.Services.Interfaces;
using Domain;
using Infrastructure;

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

        public async Task<List<User>> GetAllUsers()
        {
            _logger.LogInformation(_context.users.ToString());

            return await Task.FromResult(_context.users);
        }

        public async Task<User?> GetUser(Guid id)
        {
            var user = Task.FromResult(_context.users.FirstOrDefault(x => x.Id == id));

            return await user;
        }
    }
}
