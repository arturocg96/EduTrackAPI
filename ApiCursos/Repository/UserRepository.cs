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
        private readonly string _secretKey = string.Empty;

        public UserRepository(
            ApplicationDbContext context,
            IConfiguration config,
            UserManager<UserApp> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _secretKey = config.GetValue<string>("ApiSettings:secretKey") ?? throw new ArgumentNullException("Secret key not found in configuration");
        }

        public UserApp? GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
            return _context.UserApp.FirstOrDefault(u => u.Id == userId);
        }

        public ICollection<UserApp> GetUsers()
        {
            return _context.UserApp.OrderBy(u => u.UserName).ToList();
        }

        public bool IsUniqueUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            return !_context.UserApp.Any(u => u.UserName.ToLower() == username.ToLower());
        }

        public async Task<UserLoginResponseDto> Login(UserLoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return new UserLoginResponseDto
                {
                    Token = string.Empty,
                    User = null
                };
            }

            var user = _context.UserApp.FirstOrDefault(u => u.UserName.ToLower() == loginDto.Username.ToLower());

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
            if (registerDto == null ||
                string.IsNullOrEmpty(registerDto.Username) ||
                string.IsNullOrEmpty(registerDto.Password) ||
                string.IsNullOrEmpty(registerDto.Name))
            {
                return null;
            }

            if (!IsUniqueUser(registerDto.Username))
            {
                return null;
            }

            var user = new UserApp
            {
                UserName = registerDto.Username,
                Email = registerDto.Username,
                NormalizedEmail = registerDto.Username.ToUpper(),
                Name = registerDto.Name
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return null;
            }

            await EnsureRolesExist();

            var role = string.Equals(registerDto.Role, "Admin", StringComparison.OrdinalIgnoreCase)
                ? "Admin"
                : "User";

            await _userManager.AddToRoleAsync(user, role);

            return _mapper.Map<UserDataDto>(user);
        }

        private string GenerateJwtToken(UserApp user, IList<string> roles)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (string.IsNullOrEmpty(_secretKey))
            {
                throw new InvalidOperationException("Secret key not configured");
            }

            var key = Encoding.ASCII.GetBytes(_secretKey);
            var role = roles?.FirstOrDefault() ?? "User";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                   new Claim(ClaimTypes.Role, role)
               }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task EnsureRolesExist()
        {
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}