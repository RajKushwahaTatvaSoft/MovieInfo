using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Services.IServices
{
    public interface IMovieService
    {
        public Task<PaginatedResponse<MovieDetail>> GetTrendingMovies(int pageNumber = 1, int pageSize = 12);
        public Task<PaginatedResponse<MovieDetail>> GetCategoryMovies(string category, int pageSize, int pageNumber);
        public MovieDetail? GetMovieDetailById(int movieId);
        public APIResponse AddNewMovie(MovieDetail movieCreate);
        public APIResponse UpdateMovie(MovieDetail movieUpdate);
        public APIResponse DeleteMovie(int movieId);
        public APIResponse RestoreMovie(int movieId);
        public Task<APIResponse> GetMovies(string searchName = "", int category = 0, int pageNumber = 1, int pageSize = 10);
        public Task<APIResponse> GetDeletedMovies(string searchName = "", int category = 0, int pageNumber = 1, int pageSize = 10);
        public APIResponse GetSuggestionForMovieSearch(string searchInput);
        public IEnumerable<MovieGenre> GetMovieGenres();
        public Task<PaginatedResponse<MovieDetail>> GetMovieByName(string movieName, int pageSize = 12, int pageNumber = 1);
    }
}
