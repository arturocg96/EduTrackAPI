using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ApiCursos.Models
{
    public class UserApp : IdentityUser
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}