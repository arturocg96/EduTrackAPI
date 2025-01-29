using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiCursos.Repository.IRepository;
using ApiCursos.Models.Dtos.UserDtos;
using System.Net;
using Asp.Versioning;
using ApiCursos.Models;

namespace ApiCursos.Controllers.V1
{
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("1.0")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUser _userRepository;
        private readonly IMapper _mapper;
        private APIResponse _apiResponse;

        public UsersController(IUser userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _apiResponse = new APIResponse();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ResponseCache(CacheProfileName = "Default30Seconds")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetUsers()
        {
            var users = _userRepository.GetUsers();
            var userDtos = users.Select(_mapper.Map<UserDto>).ToList();
            return Ok(userDtos);
        }

        [AllowAnonymous]
        [HttpGet("{userId}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUser(string userId)
        {
            var user = _userRepository.GetUser(userId);
            return user == null ? NotFound() : Ok(_mapper.Map<UserDto>(user));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            if (!_userRepository.IsUniqueUser(userRegisterDto.Username))
            {
                return BadRequest(new { error = "Username already exists" });
            }

            var registeredUser = await _userRepository.Register(userRegisterDto);
            if (registeredUser == null)
            {
                return BadRequest(new { error = "Registration failed" });
            }

            return CreatedAtRoute("GetUserById", new { userId = registeredUser.ID }, registeredUser);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var loginResponse = await _userRepository.Login(userLoginDto);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                return BadRequest(new { error = "Invalid username or password" });
            }

            return Ok(loginResponse);
        }
    }
}