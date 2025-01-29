namespace ApiCursos.Models.Dtos.UserDtos
{
    public class UserLoginResponseDto
    {
        public UserDataDto? User { get; set; }
       
        public string Token { get; set; } = string.Empty;
    }
}