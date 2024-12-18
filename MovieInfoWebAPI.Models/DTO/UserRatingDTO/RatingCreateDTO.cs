namespace MovieInfoWebAPI.Models.DTO.UserRatingDTO
{
    public class RatingCreateDTO
    {
        public int MovieId { get; set; }
        public int Rating { get; set; }
        public string? Description { get; set; }
    }
}
