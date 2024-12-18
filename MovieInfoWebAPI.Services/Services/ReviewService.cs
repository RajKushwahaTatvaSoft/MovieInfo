using Microsoft.EntityFrameworkCore;
using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Models.DTO;
using MovieInfoWebAPI.Repositories.IRepository;
using MovieInfoWebAPI.Services.Constants;
using MovieInfoWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public APIResponse AddUserReview(int userId, UserRating ratingByUser)
        {
            APIResponse response = new APIResponse();

            if (!_unitOfWork.Users.Any(user => user.UserId == userId))
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages.Add(APIMessages.USER_NOT_FOUND);
                return response;
            }

            UserRating? existingReview = _unitOfWork.UserRatings.GetFirstOrDefault(rating => rating.MovieId == ratingByUser.MovieId && rating.UserId == userId);
            MovieDetail? movie = _unitOfWork.MovieDetails.GetFirstOrDefault(movie => movie.MovieId == ratingByUser.MovieId);

            if (movie == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages.Add(APIMessages.MOVIE_NOT_FOUND);
                return response;
            }

            if (existingReview != null)
            {
                response.StatusCode = HttpStatusCode.Conflict;
                response.IsSuccess = false;
                response.ErrorMessages.Add(APIMessages.REVIEW_ALREADY_EXISTS);
                return response;
            }

            decimal totalRating = movie.Rating * movie.RatingCount + ratingByUser.Rating;
            decimal updatedRating = totalRating / (movie.RatingCount + 1);

            movie.Rating = updatedRating;
            movie.RatingCount = movie.RatingCount + 1;

            _unitOfWork.MovieDetails.Update(movie);

            ratingByUser.CreatedDate = DateTime.Now;
            ratingByUser.UserId = userId;
            _unitOfWork.UserRatings.Add(ratingByUser);
            _unitOfWork.Save();

            response.Result = ratingByUser.RatingId;
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.Created;
            return response;

        }

        public APIResponse UpdateUserReview(int userId, UserRating ratingByUser)
        {
            APIResponse response = new APIResponse();

            if (!_unitOfWork.Users.Any(user => user.UserId == userId))
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages.Add(APIMessages.USER_NOT_FOUND);
                return response;
            }

            UserRating? existingReview = _unitOfWork.UserRatings.GetFirstOrDefault(rating => rating.MovieId == ratingByUser.MovieId && rating.UserId == userId);
            MovieDetail? movie = _unitOfWork.MovieDetails.GetFirstOrDefault(movie => movie.MovieId == ratingByUser.MovieId);

            if (movie == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages.Add(APIMessages.MOVIE_NOT_FOUND);
                return response;
            }

            if (existingReview == null)
            {

                response.StatusCode = HttpStatusCode.NotFound;
                response.IsSuccess = false;
                response.ErrorMessages.Add(APIMessages.REVIEW_NOT_FOUND);
                return response;

            }

            decimal totalRating = movie.Rating * movie.RatingCount - existingReview.Rating + ratingByUser.Rating;
            decimal updatedRating = totalRating / movie.RatingCount;

            movie.Rating = updatedRating;

            _unitOfWork.MovieDetails.Update(movie);

            existingReview.Rating = ratingByUser.Rating;
            existingReview.ModifiedDate = DateTime.Now;
            existingReview.Description = ratingByUser.Description;

            _unitOfWork.UserRatings.Update(existingReview);
            _unitOfWork.Save();

            response.Result = existingReview.RatingId;

            response.IsSuccess = true;

            response.StatusCode = HttpStatusCode.OK;
            response.Result = APIMessages.REVIEW_UPDATE_SUCCESS;
            return response;

        }
        public async Task<PaginatedResponse<UserRating>> GetMovieReviews(int movieId, int pageNumber = 1, int pageSize = 10)
        {

            IQueryable<UserRating> userRatings = _unitOfWork.UserRatings.Where(rating => rating.MovieId == movieId).Include(_ => _.User).AsNoTracking().OrderBy(_ => _.RatingId);

            PaginatedResponse<UserRating> paginatedResponse = await PaginatedResponse<UserRating>.CreateAsync(userRatings, pageNumber, pageSize);

            return paginatedResponse;
        }

        public UserRating? FetchUserRating(int userId, int movieId)
        {

            UserRating? userRating = _unitOfWork.UserRatings.GetFirstOrDefault(rating => rating.MovieId == movieId && rating.UserId == userId);

            return userRating;
        }
    }
}
