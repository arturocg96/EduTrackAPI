﻿using System.ComponentModel.DataAnnotations;

namespace ApiCursos.Models.Dtos.UserDtos
{
    public class UserDto
    {      
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

    }
}
