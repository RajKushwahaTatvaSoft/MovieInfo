using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieInfoWebAPI.Authorization;
using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Models.DTO.CrewDTO;
using MovieInfoWebAPI.Repositories.IRepository;
using MovieInfoWebAPI.Services.Constants;
using System.Net;

namespace MovieInfoWebAPI.Controllers
{
    [Route("MovieInfo/Crew")]
    [ApiController]
    public class CrewController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        protected APIResponse _response;
        public CrewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new APIResponse();
        }

        [HttpGet("GetDepartment")]
        public ActionResult<APIResponse> GetDepartment()
        {
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = _unitOfWork.Departments.GetAll();
            return Ok(_response);
        }


        [HttpGet("GetJobsByDeptId")]
        public ActionResult<APIResponse> GetJobsByDeptId(int deptId)
        {
            if (deptId == 0)
            {
                return BadRequest();
            }

            IEnumerable<Job> jobList = _unitOfWork.Jobs.Where(job => job.DepartmentId == deptId);
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = jobList;
            return Ok(_response);
        }

        [RoleAuthorize(ApiRole.Admin)]
        [HttpPost("AddCrewDetailToMovie/{movieId:int}")]
        public ActionResult<APIResponse> AddCrewDetailToMovie(int movieId, [FromBody] List<CrewItemDTO> crewItemDTOs)
        {
            try
            {
                MovieDetail? movieDetail = _unitOfWork.MovieDetails.GetFirstOrDefault(movie => movie.MovieId == movieId);

                if (movieDetail == null)
                {
                    return APIResponse.NotFound(APIMessages.MOVIE_NOT_FOUND);
                }

                foreach (CrewItemDTO crewItem in crewItemDTOs)
                {
                    foreach (int personId in crewItem.PersonIds)
                    {

                        Crew crew = new Crew()
                        {
                            MovieId = movieId,
                            JobId = crewItem.JobId,
                            PersonId = personId,
                        };

                        _unitOfWork.Crews.Add(crew);
                    }
                }

                _unitOfWork.Save();

                return Created("Created Uri",APIResponse.Created(APIMessages.CREW_ADDED_SUCCESS));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.InternalServerError(ex.Message));
            }
        }
    }
}
