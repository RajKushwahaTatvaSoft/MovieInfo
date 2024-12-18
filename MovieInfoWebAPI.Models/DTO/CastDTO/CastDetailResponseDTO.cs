using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Models.DTO.MovieDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Models.DTO.CastDTO
{
    public class CastDetailResponseDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string? Character { get; set; } = string.Empty;
        public int CastOrder { get; set; }
    }
}
