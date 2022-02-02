using _3DeshopAPI.Models;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
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
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<UserModel>> AddUser(NewUserModel user)
        {
            var newUser = _mapper.Map<Domain.User>(user);

            await _userService.AddUser(newUser);

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, _mapper.Map<UserModel>(newUser));
        }

        /// <summary>
        /// Removes user by given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ActionResult<UserModel>> RemoveUser(Guid id)
        {
            var user = await _userService.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            await _userService.RemoveUser(user);

            return Ok(_mapper.Map<UserModel>(user));
        }

        /// <summary>
        /// Updates user by given id and NewUserModelValues
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<UserModel>> UpdateUser(Guid id, NewUserModel user)
        {
            var updatedUser = _mapper.Map<Domain.User>(user);
            var dbUser = await _userService.GetUser(id);

            if (dbUser == null)
            {
                return NotFound();
            }

            updatedUser.Id = id;

            updatedUser = await _userService.UpdateUser(updatedUser);

            return Ok(_mapper.Map<UserModel>(updatedUser));
        }
    }
}
