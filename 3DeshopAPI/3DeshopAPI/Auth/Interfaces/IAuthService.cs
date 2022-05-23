using _3DeshopAPI.Models;
using _3DeshopAPI.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Auth.Interfaces;

public interface IAuthService
{
    Task<ActionResult<TokenModel>> UserLogin(UserLoginModel model);
}