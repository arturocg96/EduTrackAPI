namespace ApiCursos.Models.Dtos.UserDtos
{
    public class UserLoginResponseDto
    {
        public User User { get; set; }
        public  string Role { get; set; }

        public string Token { get; set; }
    }
}
