using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Models.DTO.MovieDTO
{
    public class MovieReleaseYearDTO
    {
        public int ReleaseYear { get; set; }
        public int MovieCount { get; set; }
    }
}