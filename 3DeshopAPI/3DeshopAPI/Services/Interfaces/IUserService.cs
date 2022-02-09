using _3DeshopAPI.Models.User;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();
        Task<User?> GetUser(Guid id);
        Task RegisterUser(User model);
        Task RemoveUser(User model);
        Task<IActionResult> UpdateUser(Guid id, User model);
        Task<IActionResult> ChangePassword(Guid id, UserPasswordModel model);
        Task<User?> IsUserValid(UserLoginModel model);
        Task<string?> GetUserRole(UserLoginModel model);
        User GetCurrentUser();
    }
}
