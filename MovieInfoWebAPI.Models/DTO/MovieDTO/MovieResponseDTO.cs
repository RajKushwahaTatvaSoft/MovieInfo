using Microsoft.EntityFrameworkCore;
//using MovieInfoWebAPI.DataModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MovieInfoWebAPI.Models.DTO.MovieDTO
{
    public class MovieResponseDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = null!;
        public string? ReleaseDateStr { get; set; }
        public long Revenue { get; set; }
        public int? Duration { get; set; }
        public string? Overview { get; set; }
        public decimal Popularity { get; set; }
        public string? PosterUrl { get; set; }
        public string? ImdbId { get; set; }
        public long Budget { get; set; }
        public decimal Rating { get; set; }
        public int RatingCount { get; set; }
        public List<string> GenreList { get; set; } = [];
        public List<string> CastList { get; set; } = [];
        public List<string> CrewList { get; set; } = [];
        public string? Director { get; set; }
        public string? Writer { get; set; }
        public string? Actors { get; set; }

    }
}
