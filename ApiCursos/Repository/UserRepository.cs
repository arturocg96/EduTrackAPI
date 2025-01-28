using ApiCursos.Data;
using ApiCursos.Models;
using ApiCursos.Models.Dtos.UserDtos;
using ApiCursos.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace ApiCursos.Repository
{
    public class UserRepository : IUser
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;

        public UserRepository (ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            secretKey = config.GetValue<string>("ApiSettings:secretKey");
        }

        public User GetUser(int userId)
        {
            return _db.User.FirstOrDefault(c => c.Id == userId);
        }

        public ICollection<User> GetUsers()
        {
            return _db.User.OrderBy(c => c.Username).ToList();
        }

        public bool IsUniqueUser(string username)
        {
            var userDB = _db.User.FirstOrDefault(u => u.Username == username);

            if (userDB == null)
            {
                return true;
            }
            return false;

        }

        public async Task<UserLoginResponseDto> Login(UserLoginDto UserLoginDto)
        {
            var encryptedPassword = obtainmd5(UserLoginDto.Password);
            var user = _db.User.FirstOrDefault(
                u => u.Username.ToLower() == UserLoginDto.Username.ToLower() && u.Password == encryptedPassword);

            if (user == null)
            {
                return new UserLoginResponseDto()
                {
                    Token = "",
                    User = null
                };
            }

            var manageToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }), 
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = manageToken.CreateToken(tokenDescriptor);

            UserLoginResponseDto userLoginResponseDto = new UserLoginResponseDto()
            {
                Token = manageToken.WriteToken(token),
                User = user,
                Role = user.Role 
            };


            return userLoginResponseDto;
        }

        public async Task<User> Register(UserRegisterDto UserRegisterDto)
        {
            var encryptedPassword = obtainmd5(UserRegisterDto.Password);

            User user = new User()
            {
                Username = UserRegisterDto.Username,
                Password = encryptedPassword,
                Name = UserRegisterDto.Name,
                Role = string.IsNullOrEmpty(UserRegisterDto.Role) ? "User" : UserRegisterDto.Role 
            };

            _db.User.Add(user);
            await _db.SaveChangesAsync();
            user.Password = encryptedPassword;
            return user;
        }


        // obtainmd5

        public static string obtainmd5(string value)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(value);
            data = x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++) 
                resp += data[i].ToString("x2").ToLower();
            return resp;
        }


    }
}
    