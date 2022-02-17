using _3DeshopAPI.Auth.Interfaces;
using _3DeshopAPI.Models;
using _3DeshopAPI.Models.User;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UserController(IUserService userService, IAuthService authService, IMapper mapper, IConfiguration config)
        {
            _userService = userService;
            _authService = authService;
            _mapper = mapper;
            _config = config;
        }

        /// <summary>
        /// Returns user list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<UserModel>>> GetUsers()
        {
            var response = await _userService.GetAllUsers();

            return Ok(response.Select(x => _mapper.Map<UserModel>(x)));
        }

        /// <summary>
        /// Gets user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(Guid id)
        {
            var response = await _userService.GetUser(id);

            return response != null ? Ok(_mapper.Map<UserModel>(response)) : NotFound();
        }

        /// <summary>
        /// Adds user by given NewUserModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<UserModel>> RegisterUser(UserRegisterModel model)
        {
            var response = _mapper.Map<Domain.User>(model);

            await _userService.RegisterUser(response);

            return CreatedAtAction(nameof(GetUser), new { id = response.Id }, _mapper.Map<UserModel>(response));
        }

        /// <summary>
        /// Removes user by given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult<UserModel>> RemoveUser(Guid id)
        {
            var response = await _userService.GetUser(id);

            if (response == null)
            {
                return NotFound();
            }

            await _userService.RemoveUser(response);

            return Ok(_mapper.Map<UserModel>(response));
        }

        /// <summary>
        /// Updates user by given id and UserModel values
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateUser(Guid id, UserModel model)
        {
            var response = _mapper.Map<Domain.User>(model);

            return await _userService.UpdateUser(id, response);
        }

        /// <summary>
        /// Changes user password, takes old password and new password from body
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("{id}/change-password")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] UserPasswordModel model)
        {
            return await _userService.ChangePassword(id, model);
        }

        /// <summary>
        /// Gets users JWT token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("authenticate")]
        public async Task<ActionResult<TokenModel>> UserLogin([FromBody] UserLoginModel model)
        {
            return await _authService.UserLogin(model);
        }


        /// <summary>
        /// Returns user details
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-current-user")]
        public IActionResult GetCurrentUser()
        {
            return Ok(_userService.GetCurrentUser());
        }
    }
}
