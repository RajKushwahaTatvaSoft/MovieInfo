using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Models.DTO;
using MovieInfoWebAPI.Repositories.IRepository;
using MovieInfoWebAPI.Services.Constants;
using MovieInfoWebAPI.Services.IServices;
using MovieInfoWebAPI.Services.Utilities;
using System.Net;

namespace MovieInfoWebAPI.Services.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MovieService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PaginatedResponse<MovieDetail>> GetTrendingMovies(int pageNumber = 1, int pageSize = 12)
        {

            IQueryable<MovieDetail> movieList = _unitOfWork.MovieDetails.Where(movie => movie.Popularity > 20).OrderByDescending(_ => _.Popularity);

            PaginatedResponse<MovieDetail> response = await PaginatedResponse<MovieDetail>.CreateAsync(movieList, pageNumber, pageSize);

            return response;
        }

        public async Task<PaginatedResponse<MovieDetail>> GetCategoryMovies(string category, int pageSize, int pageNumber)
        {

            if (category.ToLower().Equals("Trending".ToLower()))
            {
                return await GetTrendingMovies(pageNumber, pageSize);
            }

            MovieGenre? genre = _unitOfWork.MovieGenres.GetAll().Include(_ => _.MovieGenreMappings).ThenInclude(_ => _.Movie).FirstOrDefault(genre => genre.GenreName.ToLower().Equals(category.ToLower()));

            if (genre == null)
            {
                return null;
            }

            IEnumerable<MovieGenreMapping> list = genre.MovieGenreMappings;
            int totalCount = list.Count();

            list = list.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            List<MovieDetail> movieList = new List<MovieDetail>();
            foreach (MovieGenreMapping mapping in list)
            {
                mapping.Movie.UserRatings = [];
                mapping.Movie.MovieGenreMappings = [];
                movieList.Add(mapping.Movie);
            }

            PaginatedResponse<MovieDetail> response = new PaginatedResponse<MovieDetail>(movieList, totalCount, pageNumber, pageSize);

            return response;
        }

        public MovieDetail? GetMovieDetailById(int movieId)
        {

            MovieDetail? movie = _unitOfWork.MovieDetails.GetAll().Include(_ => _.MovieGenreMappings).ThenInclude(_ => _.Genre).Include(_ => _.Casts).ThenInclude(_ => _.Person).Include(_ => _.Crews).ThenInclude(_ => _.Person).FirstOrDefault(movie => movie.MovieId == movieId);

            if (movie == null)
            {
                return null;
            }

            return movie;
        }

        public APIResponse AddNewMovie(MovieDetail movieCreate)
        {
            APIResponse response = new APIResponse();
            if (_unitOfWork.MovieDetails.Any(movie => movie.ImdbId == movieCreate.ImdbId))
            {
                response.StatusCode = HttpStatusCode.Conflict;
                response.IsSuccess = false;
                response.ErrorMessages.Add(APIMessages.MOVIE_ALREADY_EXISTS);
                return response;
            }

            Random rand = new Random();
            movieCreate.Rating = 0;
            movieCreate.RatingCount = 0;

            // TODO: Find some good metric or input for popularity
            movieCreate.Popularity = Convert.ToDecimal(RandomHelper.NextDoubleFromRange(1, 300));

            _unitOfWork.MovieDetails.Add(movieCreate);
            _unitOfWork.Save();

            response.StatusCode = HttpStatusCode.Created;
            response.IsSuccess = true;
            response.Result = movieCreate.MovieId;
            return response;
        }

        public APIResponse UpdateMovie(MovieDetail movieUpdate)
        {
            APIResponse response = new APIResponse();


            MovieDetail? movieFromDb = _unitOfWork.MovieDetails.GetFirstOrDefault(movie => movie.MovieId == movieUpdate.MovieId);

            if (movieFromDb == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages.Add(APIMessages.MOVIE_NOT_FOUND);
                return response;
            }

            movieFromDb.Actors = movieUpdate.Actors;
            movieFromDb.Writer = movieUpdate.Writer;
            movieFromDb.Director = movieUpdate.Director;
            movieFromDb.Title = movieUpdate.Title;
            movieFromDb.ReleaseDate = movieUpdate.ReleaseDate;
            movieFromDb.Revenue = movieUpdate.Revenue;
            movieFromDb.Budget = movieUpdate.Budget;
            movieFromDb.Duration = movieUpdate.Duration;
            movieFromDb.PosterUrl = movieUpdate.PosterUrl;

            _unitOfWork.MovieDetails.Update(movieFromDb);
            _unitOfWork.Save();


            response.StatusCode = HttpStatusCode.OK;
            response.Result = APIMessages.MOVIE_UPDATE_SUCCESS;
            return response;
        }

        public APIResponse DeleteMovie(int movieId)
        {
            APIResponse response = new APIResponse();
            MovieDetail? movie = _unitOfWork.MovieDetails.GetFirstOrDefault(movie => movie.MovieId == movieId);
            if (movie == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages.Add(APIMessages.MOVIE_NOT_FOUND);
                return response;
            }

            movie.IsDeleted = true;
            _unitOfWork.MovieDetails.Update(movie);
            _unitOfWork.Save();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        public APIResponse RestoreMovie(int movieId)
        {
            APIResponse response = new APIResponse();
            MovieDetail? movie = _unitOfWork.MovieDetails.GetDeletedMovie(movie => movie.MovieId == movieId);
            if (movie == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages.Add(APIMessages.MOVIE_NOT_FOUND);
                return response;
            }

            movie.IsDeleted = false;
            _unitOfWork.MovieDetails.Update(movie);
            _unitOfWork.Save();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        public async Task<APIResponse> GetMovies(string searchName = "", int category = 0, int pageNumber = 1, int pageSize = 10)
        {
            APIResponse response = new APIResponse();


            IQueryable<MovieDetail> movies = from movie in _unitOfWork.MovieDetails.GetAll()
                                             where (category == 0 || movie.MovieGenreMappings.Any(genreMap => genreMap.GenreId == category))
                                             && (string.IsNullOrEmpty(searchName) || movie.Title.ToLower().Contains(searchName.ToLower()))
                                             orderby movie.MovieId
                                             select movie;

            PaginatedResponse<MovieDetail> pagedMovieList = await PaginatedResponse<MovieDetail>.CreateAsync(movies, pageNumber, pageSize);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = pagedMovieList;
            return response;
        }

        public async Task<APIResponse> GetDeletedMovies(string searchName = "", int category = 0, int pageNumber = 1, int pageSize = 10)
        {
            APIResponse response = new APIResponse();


            IQueryable<MovieDetail> movies = from movie in _unitOfWork.MovieDetails.GetDeleteMovies()
                                             where (category == 0 || movie.MovieGenreMappings.Any(genreMap => genreMap.GenreId == category))
                                             && (string.IsNullOrEmpty(searchName) || movie.Title.ToLower().Contains(searchName.ToLower()))
                                             orderby movie.MovieId
                                             select movie;

            PaginatedResponse<MovieDetail> pagedMovieList = await PaginatedResponse<MovieDetail>.CreateAsync(movies, pageNumber, pageSize);

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = pagedMovieList;
            return response;
        }

        public APIResponse GetSuggestionForMovieSearch(string searchInput)
        {
            APIResponse response = new APIResponse();

            int pageSize = 5;

            var moviesList = _unitOfWork.MovieDetails.Where(movie => movie.Title != null && movie.Title.ToLower().Contains(searchInput.ToLower())).OrderBy(_ => _.MovieId).Take(pageSize).Select(_ => new { _.MovieId, _.Title });

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = moviesList;

            return response;
        }

        public IEnumerable<MovieGenre> GetMovieGenres()
        {
            return _unitOfWork.MovieGenres.GetAll();
        }

        public async Task<PaginatedResponse<MovieDetail>> GetMovieByName(string movieName, int pageSize = 12, int pageNumber = 1)
        {

            IQueryable<MovieDetail> list = from movie in _unitOfWork.MovieDetails.GetAll()
                                           where movie.Title.ToLower().Contains(movieName.ToLower())
                                           select movie
                                             ;

            PaginatedResponse<MovieDetail> pagedList = await PaginatedResponse<MovieDetail>.CreateAsync(list, pageNumber, pageSize);

            return pagedList;
        }
    }
}
