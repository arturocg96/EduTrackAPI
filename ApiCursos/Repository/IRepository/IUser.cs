using ApiCursos.Models;
using ApiCursos.Models.Dtos.UserDtos;

namespace ApiCursos.Repository.IRepository
{
    public interface IUser
    {
        ICollection<User> GetUsers();
        User GetUser(int userId);
        bool IsUniqueUser(string username);
        Task<UserLoginResponseDto> Login(UserLoginDto UserLoginDto);
        Task<User> Register(UserRegisterDto UserRegisterDto);
    }
}
