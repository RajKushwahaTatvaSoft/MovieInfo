using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieInfoWebAPI.Authorization;
using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Repositories.IRepository;
using MovieInfoWebAPI.Services.Constants;
using System.Net;

namespace MovieInfoWebAPI.Controllers
{
    [Route("MovieInfo/Person")]
    [ApiController]
    public class PersonController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        protected APIResponse _response;
        public PersonController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _response = new APIResponse();
        }

        [RoleAuthorize(ApiRole.Admin)]
        [HttpGet("FetchPersonByName")]
        public ActionResult<APIResponse> FetchPersonList(string searchInput = "")
        {
            if (string.IsNullOrEmpty(searchInput))
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Empty Not Allowed");
                return NotFound(_response);
            }

            string validSearch = searchInput.Trim().ToLower();

            int pageSize = 5;

            IEnumerable<Person> personList = _unitOfWork.People.Where(person => person.PersonName.Trim().ToLower().Contains(validSearch)).Take(pageSize);

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = personList;
            return Ok(_response);
        }

        [RoleAuthorize(ApiRole.AllRoles)]
        [HttpGet("FetchPersonById/{personId:int}")]
        public ActionResult<APIResponse> FetchPersonById(int personId)
        {
            try
            {
                Person? person = _unitOfWork.People.GetFirstOrDefault(person=> person.PersonId == personId);
                if (person == null)
                {
                    return NotFound(APIResponse.NotFound(APIMessages.PERSON_NOT_FOUND));
                }

                return Ok(APIResponse.OK(person));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,APIResponse.InternalServerError(ex.Message));
            }
        }
    }
}