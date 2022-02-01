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

        [HttpGet]
        public async Task<ActionResult<List<UserModel>>> GetUsers()
        {
            var response = await _userService.GetAllUsers();

            return Ok(response.Select(x => _mapper.Map<UserModel>(x)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserModel>> GetUser(Guid id)
        {
            var response = await _userService.GetUser(id);

            return response != null ? Ok(_mapper.Map<UserModel>(response)) : NotFound();
        }
    }
}
