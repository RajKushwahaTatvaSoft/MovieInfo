﻿using System.ComponentModel.DataAnnotations;

namespace MovieInfoWebAPI.Models.DTO.UserDTO
{
    public class UserCreateDTO
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
