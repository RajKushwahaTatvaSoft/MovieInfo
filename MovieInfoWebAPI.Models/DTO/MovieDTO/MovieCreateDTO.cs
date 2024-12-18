using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MovieInfoWebAPI.Models.DTO.MovieDTO
{
    public class MovieCreateDTO
    {
        public string Title { get; set; } = string.Empty;
        public DateOnly ReleaseDate { get; set; }
        public int Duration { get; set; }
        public string PosterUrl { get; set; } = string.Empty;
        public string ImdbId { get; set; } = string.Empty;
        public long? Revenue { get; set; }
        public long? Budget { get; set; }
        public string? OverView { get; set; } = string.Empty;
        public string? Plot { get; set; } = string.Empty;
        public string? Writer { get; set; } = string.Empty;
        public string? Actor { get; set; } = string.Empty;
        public string? Director { get; set; } = string.Empty;
    }
}
