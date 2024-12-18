using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieInfoWebAPI.Authorization;
using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.CustomTableModels;
using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Models.DTO.CastDTO;
using MovieInfoWebAPI.Models.DTO.PersonDTO;
using MovieInfoWebAPI.Repositories.IRepository;
using MovieInfoWebAPI.Services.Constants;

namespace MovieInfoWebAPI.Controllers
{
    [Route("MovieInfo/Cast")]
    [ApiController]
    public class CastController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CastController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [RoleAuthorize(ApiRole.Admin)]
        [HttpPost("AddCastDetailToMovie/{movieId:int}")]
        public ActionResult<APIResponse> AddCastDetailToMovie(int movieId, [FromBody] List<CastItemDTO> castItemDTOs)
        {
            try
            {
                MovieDetail? movieDetail = _unitOfWork.MovieDetails.GetFirstOrDefault(movie => movie.MovieId == movieId);

                if (movieDetail == null)
                {
                    return APIResponse.NotFound(APIMessages.MOVIE_NOT_FOUND);
                }

                foreach (CastItemDTO castItem in castItemDTOs)
                {
                    Cast cast = new Cast()
                    {
                        MovieId = movieId,
                        PersonId = castItem.PersonId,
                        Character = castItem.CharacterName,
                    };

                    _unitOfWork.Casts.Add(cast);

                }

                _unitOfWork.Save();

                return Created("Created Uri", APIResponse.Created(APIMessages.CREW_ADDED_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.InternalServerError(ex.Message));
            }
        }

        [HttpGet("GetPaginatedCastList")]
        public async Task<ActionResult<APIResponse>> GetPaginatedCastList(string searchActorName = "", int pageNumber = 1, int pageSize = 12)
        {
            try
            {
                PaginatedResponse<CastItem> castItems = await _unitOfWork.Casts.GetCastItems(searchActorName, pageNumber, pageSize);

                if (castItems.TotalCount == 0)
                {
                    return NotFound();
                }

                return Ok(APIResponse.OK(castItems));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.InternalServerError(ex.Message));
            }
        }
        
        
        [HttpGet("FetchSuggestionsForActor")]
        public async Task<ActionResult<APIResponse>> FetchSuggestionsForActor(string searchActorName = "")
        {
            try
            {
                IEnumerable<PersonSearchItemDTO> castItems = await _unitOfWork.Casts.GetCastSuggestions(searchActorName);

                return Ok(APIResponse.OK(castItems));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.InternalServerError(ex.Message));
            }
        }

        [HttpGet("GetMoviesStaredIn/{personId:int}")]
        public ActionResult<APIResponse> GetMoviesStaredIn(int personId)
        {
            try
            {
                IEnumerable<Cast> casts = _unitOfWork.Casts.Where(cast => cast.PersonId == personId).Include(_ => _.Movie).ToList();
                IEnumerable<CastDetailResponseDTO> castResponse = _mapper.Map<IEnumerable<CastDetailResponseDTO>>(casts);
                return Ok(APIResponse.OK(castResponse.ToList()));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.InternalServerError(ex.Message));
            }
        }
    }
}
