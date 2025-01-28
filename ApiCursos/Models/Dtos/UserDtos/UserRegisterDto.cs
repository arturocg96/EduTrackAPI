using System.ComponentModel.DataAnnotations;

namespace ApiCursos.Models.Dtos.UserDtos
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "The user is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage ="The name is required")]
        public string Name { get; set; }        

        [Required(ErrorMessage ="The password is required")]
        public string Password { get; set; }  
        public string Role { get; set; }
    }
}
