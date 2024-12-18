using Microsoft.EntityFrameworkCore;

namespace MovieInfoWebAPI.Models.DTO.UserRatingDTO
{
    public class RatingResponseDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string UserProfileUrl { get; set; }
        public string? Description { get; set; }

        [Precision(4, 2)]
        public decimal Rating { get; set; }
        public string ReviewDate { get; set; } = string.Empty;
        public bool IsModified { get; set; } = false;
    }
}
