using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiCursos.Repository.IRepository;
using ApiCursos.Models.Dtos.UserDtos;
using ApiCursos.Models.Dtos.CategoryDtos;
using ApiCursos.Models;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;

namespace ApiCursos.Controllers
{
    [Route("api/v{version:apiVersion}/users")]
    [ApiVersion("1.0")]
    [ApiController]

    public class UsersController : ControllerBase
    {
        private readonly IUser _usRepo;
        protected APIResponse _APIresponse;
        private readonly IMapper _mapper;

        public UsersController(IUser usRepo, IMapper mapper)
        {
            _usRepo = usRepo;
            _mapper = mapper;
            this._APIresponse = new APIResponse();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]       
        [ResponseCache(CacheProfileName= "Default30Seconds")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public IActionResult GetUsers()
        {
            var usersList = _usRepo.GetUsers();
            var usersDtoList = new List<UserDto>();

            foreach (var user in usersList)
            {
                usersDtoList.Add(_mapper.Map<UserDto>(user));
            }
            return Ok(usersDtoList);
        }

        [AllowAnonymous]
        [HttpGet("{userId:int}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUser(int userId)
        {
            var userItem = _usRepo.GetUser(userId);

            if (userItem == null)
            {
                return NotFound();
            }

            var userItemDto = _mapper.Map<UserDto>(userItem);

            return Ok(userItemDto);
        }

        [AllowAnonymous]
        [HttpPost("register")]       
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
           bool uniqueUserNameValidator = _usRepo.IsUniqueUser(userRegisterDto.Username);
            if (!uniqueUserNameValidator)
            {
                _APIresponse.StatusCode = HttpStatusCode.BadRequest;
                _APIresponse.IsSuccess=false;
                _APIresponse.ErrorMessages.Add("The username already exists");
                return BadRequest(_APIresponse);
            }

            var user = await _usRepo.Register(userRegisterDto);
            if (user == null)
            {
                _APIresponse.StatusCode = HttpStatusCode.BadRequest;
                _APIresponse.IsSuccess = false;
                _APIresponse.ErrorMessages.Add("Register error");
                return BadRequest(_APIresponse);
            }

            _APIresponse.StatusCode=HttpStatusCode.OK;
            _APIresponse.IsSuccess = true;
            return Ok(_APIresponse);

        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var loginResponse = await _usRepo.Login(userLoginDto);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _APIresponse.StatusCode = HttpStatusCode.BadRequest;
                _APIresponse.IsSuccess = false;
                _APIresponse.ErrorMessages.Add("The username or password are incorrect.");
                return BadRequest(_APIresponse);
            }

           _APIresponse.StatusCode = HttpStatusCode.OK;
            _APIresponse.IsSuccess = true;
            _APIresponse.Result = loginResponse;
            return Ok(_APIresponse);

        }

    }
}
