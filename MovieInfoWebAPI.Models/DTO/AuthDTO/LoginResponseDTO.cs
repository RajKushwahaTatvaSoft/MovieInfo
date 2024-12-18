using MovieInfoWebAPI.Models.DataModels;

namespace MovieInfoWebAPI.Models.DTO.AuthDTO
{
    public class LoginResponseDTO
    {
        public User LoggedInUser { get; set; }
        public string Token { get; set; }
        public string ProfileUrl { get; set; }

    }
}
