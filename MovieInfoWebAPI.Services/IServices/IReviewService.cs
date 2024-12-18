using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Services.IServices
{
    public interface IReviewService
    {
        public APIResponse AddUserReview(int userId, UserRating ratingByUser);
        public APIResponse UpdateUserReview(int userId, UserRating ratingByUser);
        public  Task<PaginatedResponse<UserRating>> GetMovieReviews(int movieId, int pageNumber = 1, int pageSize = 10);
        public UserRating? FetchUserRating(int userId, int movieId);
    }
}
