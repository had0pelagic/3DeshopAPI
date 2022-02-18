using _3DeshopAPI.Auth.Interfaces;
using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Models;
using _3DeshopAPI.Models.User;
using _3DeshopAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace _3DeshopAPI.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public AuthService(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }

        /// <summary>
        /// Get users JWT token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult<TokenModel>> UserLogin(UserLoginModel model)
        {
            var user = await _userService.IsUserValid(model);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.UserRole),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = GetToken(authClaims);

            return new CreatedResult(nameof(TokenModel), new TokenModel { Token = new JwtSecurityTokenHandler().WriteToken(token), ExpirationDate = token.ValidTo });
        }

        /// <summary>
        /// Creates JWT token
        /// </summary>
        /// <param name="authClaims"></param>
        /// <returns></returns>
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(int.Parse(_config["JWT:ValidTime"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}
