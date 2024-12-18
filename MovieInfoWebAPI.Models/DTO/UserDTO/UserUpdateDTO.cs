using System.ComponentModel.DataAnnotations;

namespace MovieInfoWebAPI.Models.DTO.UserDTO
{
    public class UserUpdateDTO
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required]
        public string UserEmail { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
