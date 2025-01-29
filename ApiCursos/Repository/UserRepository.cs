using ApiCursos.Data;
using ApiCursos.Models;
using ApiCursos.Models.Dtos.UserDtos;
using ApiCursos.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiCursos.Repository
{
    public class UserRepository : IUser
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UserApp> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly string _secretKey;

        public UserRepository(
            ApplicationDbContext context,
            IConfiguration config,
            UserManager<UserApp> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _secretKey = config.GetValue<string>("ApiSettings:secretKey");
        }

        public UserApp? GetUser(string userId) =>
            _context.UserApp.FirstOrDefault(u => u.Id == userId);

        public ICollection<UserApp> GetUsers() =>
            _context.UserApp.OrderBy(u => u.UserName).ToList();

        public bool IsUniqueUser(string username)
        {
            return _context.UserApp.All(u => u.UserName.ToLower() != username.ToLower());
        }

        public async Task<UserLoginResponseDto> Login(UserLoginDto loginDto)
        {
            var user = _context.UserApp.FirstOrDefault(
                u => u.UserName.ToLower() == loginDto.Username.ToLower());

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return new UserLoginResponseDto
                {
                    Token = string.Empty,
                    User = null
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            return new UserLoginResponseDto
            {
                Token = token,
                User = _mapper.Map<UserDataDto>(user)
            };
        }

        public async Task<UserDataDto?> Register(UserRegisterDto registerDto)
        {
            if (!IsUniqueUser(registerDto.Username))
                return null;

            var user = new UserApp
            {
                UserName = registerDto.Username,
                Email = registerDto.Username,
                NormalizedEmail = registerDto.Username.ToUpper(),
                Name = registerDto.Name
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return null;

            await EnsureRolesExist();
            if (registerDto.Role == "Admin")
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return _mapper.Map<UserDataDto>(user);
        }

        private string GenerateJwtToken(UserApp user, IList<string> roles)
        {
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private async Task EnsureRolesExist()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
        }
    }
}