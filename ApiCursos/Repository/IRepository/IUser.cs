using ApiCursos.Models;
using ApiCursos.Models.Dtos.UserDtos;

namespace ApiCursos.Repository.IRepository
{
    public interface IUser
    {
        ICollection<UserApp> GetUsers();
        UserApp? GetUser(string userId);
        bool IsUniqueUser(string username);
        Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
        Task<UserDataDto?> Register(UserRegisterDto userRegisterDto);
    }
}