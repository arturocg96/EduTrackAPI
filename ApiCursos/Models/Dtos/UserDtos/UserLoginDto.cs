using System.ComponentModel.DataAnnotations;

namespace ApiCursos.Models.Dtos.UserDtos
{
    public class UserLoginDto
    {

        [Required(ErrorMessage = "The user is required.")]
        public string Username { get; set; }
        

        [Required(ErrorMessage = "The password is required")]
        public string Password { get; set; }
    }
}
