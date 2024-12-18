using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieInfoWebAPI.Authorization;
using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.DataContext;
using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Models.DTO.UserRatingDTO;
using MovieInfoWebAPI.Repositories.IRepository;
using MovieInfoWebAPI.Services.Constants;
using MovieInfoWebAPI.Services.IServices;
using MovieInfoWebAPI.Services.Services.Helper.Interface;
using System.Net;

namespace MovieInfoWebAPI.Controllers
{
    [Route("MovieInfo/Review")]
    [ApiController]
    public class ReviewController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        protected APIResponse _response;
        private readonly IReviewService _reviewService;


        public ReviewController(IConfiguration configuration, IMapper mapper, IJwtService jwtService, IReviewService reviewService)
        {
            _mapper = mapper;
            _config = configuration;
            _jwtService = jwtService;
            _response = new();
            _reviewService = reviewService;
        }

        [RoleAuthorize(ApiRole.AllRoles)]
        [HttpGet("GetMovieReviews/{movieId:int}")]
        public async Task<ActionResult<APIResponse>> GetMovieReviews(int movieId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {

                PaginatedResponse<UserRating> userRatings = await _reviewService.GetMovieReviews(movieId, pageNumber, pageSize);

                PaginatedResponse<RatingResponseDTO> paginatedResponse = _mapper.Map<PaginatedResponse<RatingResponseDTO>>(userRatings);

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = paginatedResponse;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = [ex.Message];
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

        }

        [RoleAuthorize(ApiRole.User)]
        [HttpPost("AddReview")]
        public ActionResult<APIResponse> AddUserReview([FromBody] RatingCreateDTO ratingByUser)
        {

            try
            {
                int userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);

                if (userId == 0)
                {
                    return BadRequest();
                }

                UserRating userRating = _mapper.Map<UserRating>(ratingByUser);
                APIResponse serviceResponse = _reviewService.AddUserReview(userId, userRating);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(serviceResponse);
                }

                return StatusCode((int)serviceResponse.StatusCode, serviceResponse);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = [ex.Message];
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

        }

        [RoleAuthorize(ApiRole.User)]
        [HttpPut("UpdateReview/{reviewId:int}")]
        public ActionResult<APIResponse> UpdateUserReview(int reviewId, [FromBody] RatingCreateDTO ratingByUser)
        {

            try
            {
                int userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);

                if (userId == 0 || reviewId == 0)
                {
                    return BadRequest();
                }

                UserRating userRating = _mapper.Map<UserRating>(ratingByUser);
                APIResponse serviceResponse = _reviewService.UpdateUserReview(userId, userRating);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(serviceResponse);
                }

                return StatusCode((int)serviceResponse.StatusCode, serviceResponse);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = [ex.Message];
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }

        }

        [RoleAuthorize(ApiRole.User)]
        [HttpGet("GetUserSelfReview/{movieId:int}")]
        public ActionResult<APIResponse> GetSelfReview(int movieId)
        {
            try
            {
                int userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);

                if (userId == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add(APIMessages.USER_NOT_FOUND);
                    return BadRequest(_response);
                }

                UserRating? userRatings = _reviewService.FetchUserRating(userId, movieId);

                if (userRatings == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add(APIMessages.REVIEW_NOT_FOUND);
                    return NotFound(_response);
                }

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = userRatings;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = [ex.Message];
                return StatusCode(StatusCodes.Status500InternalServerError, _response);
            }
        }

    }
}
