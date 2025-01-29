using Microsoft.AspNetCore.Identity;

namespace ApiCursos.Models
{
    public class UserApp : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
    }
}