using Domain;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User?> GetUser(Guid id);
        Task AddUser(User user);
        Task RemoveUser(User user);
        Task<User?> UpdateUser(User user);
    }
}
