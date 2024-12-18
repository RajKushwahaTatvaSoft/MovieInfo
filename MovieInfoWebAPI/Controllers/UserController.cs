using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieInfoWebAPI.Models.DataModels;
using System.Net;
using AutoMapper;
using MovieInfoWebAPI.Services.Services.Helper.Interface;
using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.DataContext;
using MovieInfoWebAPI.Services.Constants;
using MovieInfoWebAPI.Services.Utilities;
using MovieInfoWebAPI.Repositories.IRepository;
using MovieInfoWebAPI.Authorization;
using MovieInfoWebAPI.Services.IServices;
using MovieInfoWebAPI.Models.DTO.UserDTO;
using MovieInfoWebAPI.Models.DTO.AuthDTO;

namespace MovieInfoWebAPI.Controllers
{
    [Route("MovieInfo/User")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;
        public UserController(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IJwtService jwtService, IUserService userService, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = configuration;
            _jwtService = jwtService;
            _userService = userService;
            _environment = webHostEnvironment;
        }

        [HttpPost("UserLogin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<APIResponse> LoginUser([FromBody] LoginDTO loginDetail)
        {
            try
            {
                string hashedPass = AuthHelper.GenerateSHA256(loginDetail.Password);

                User? user = _unitOfWork.Users.GetFirstOrDefault(user => user.Email.Equals(loginDetail.UserName) && user.PassHash == hashedPass);

                if (user == null)
                {
                    return NotFound(APIResponse.NotFound(APIMessages.USER_NOT_FOUND));
                }

                SessionUser sessionUser = new()
                {
                    Email = user.Email,
                    UserFullName = $"{user.FirstName} {user.LastName}",
                    UserId = user.UserId,
                    RoleId = user.RoleId,
                };

                string token = _jwtService.GenerateJwtToken(sessionUser);
                user.PassHash = "";

                
                LoginResponseDTO loginResponse = new LoginResponseDTO()
                {
                    Token = token,
                    LoggedInUser = user,
                    ProfileUrl = ImageHelper.GetProfileURLFromUser(user),
                };

                return Ok(APIResponse.Success(HttpStatusCode.OK, loginResponse));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetRoles")]
        public ActionResult<APIResponse> GetRoles()
        {
            try
            {
                IEnumerable<Role> userRoles = _unitOfWork.Roles.GetAll();

                return Ok(APIResponse.Success(HttpStatusCode.OK, userRoles));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("UploadUserProfilePhoto")]
        public async Task<ActionResult> UploadUserProfile([FromForm] ProfileUpdateDTO profileUpdate)
            {

            User? user = _unitOfWork.Users.GetFirstOrDefault(x => x.UserId == profileUpdate.UserId);
            if (user == null)
            {
                return NotFound(APIResponse.NotFound(APIMessages.USER_NOT_FOUND));
            }

            if (profileUpdate.ProfilePhotoFile == null)
            {
                return BadRequest(APIResponse.Error(HttpStatusCode.BadRequest, "Please select a file."));
            }

            string extension = Path.GetExtension(profileUpdate.ProfilePhotoFile.FileName);

            await FileHelper.InsertUserProfilePhoto(profileUpdate.ProfilePhotoFile, _environment.WebRootPath, profileUpdate.UserId);
            user.ProfilePhotoName = $"ProfilePhoto{extension}";
            _unitOfWork.Users.Update(user);
            _unitOfWork.Save();


            return Ok();
        }

        [HttpPut("UpdateUserProfileDetail")]
        public ActionResult<APIResponse> UpdateUserProfileDetail([FromBody] UserUpdateDTO userUpdate)
        {
            try
            {
                User? user = _unitOfWork.Users.GetFirstOrDefault(user => user.UserId == userUpdate.UserId);

                if (user == null)
                {
                    return APIResponse.NotFound(APIMessages.USER_NOT_FOUND);
                }

                user.FirstName = userUpdate.FirstName;
                user.LastName = userUpdate.LastName;
                user.Email = userUpdate.UserEmail;

                _unitOfWork.Users.Update(user);
                _unitOfWork.Save();

                return APIResponse.OK();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.Error(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetUserProfile")]
        public ActionResult GetUserProfile()
        {
            try
            {
                int userId = Convert.ToInt32(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);

                if (userId == 0)
                {
                    return BadRequest();
                }

                User? user = _unitOfWork.Users.GetAll().Include(_ => _.Role).FirstOrDefault(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound();
                }

                UserResponseDTO response = _mapper.Map<UserResponseDTO>(user);
                if (user.ProfilePhotoName != null)
                {
                    response.ProfilePath = $"https://localhost:7048/ServerData/User/{user.UserId}/{user.ProfilePhotoName}";
                }
                //_response.IsSuccess = true;
                //_response.StatusCode = HttpStatusCode.OK;
                //_response.Result = user;
                return Ok(APIResponse.Success(HttpStatusCode.OK, response));



            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("GetUserById/{userId:int}")]
        public ActionResult<APIResponse> GetUserById(int userId)
        {
            try
            {
                User? user = _unitOfWork.Users.GetFirstOrDefault(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound();
                }

                //_response.IsSuccess = true;
                //_response.StatusCode = HttpStatusCode.OK;
                //_response.Result = user;
                return Ok(APIResponse.Success(HttpStatusCode.OK, user));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPost("AddUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<APIResponse> CreateNewUser([FromBody] UserCreateDTO userDetail)
        {
            try
            {
                User mappedUser = _mapper.Map<User>(userDetail);

                mappedUser.RoleId = (int)ApiRole.User;
                mappedUser.CreatedDate = DateTime.Now;
                mappedUser.PassHash = AuthHelper.GenerateSHA256(userDetail.Password);

                _unitOfWork.Users.Add(mappedUser);
                _unitOfWork.Save();

                //_response.IsSuccess = true;
                //_response.StatusCode = HttpStatusCode.Created;
                //_response.Result = mappedUser.UserId;
                return Created($"${nameof(GetUserById)}/${mappedUser.UserId}", APIResponse.Success(HttpStatusCode.Created, mappedUser.UserId));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [RoleAuthorize(ApiRole.Admin)]
        [HttpGet("GetUsers")]
        public async Task<ActionResult<APIResponse>> GetUsers(int pageNumber = 1, int pageSize = 20)
        {
            IQueryable<User> users = _unitOfWork.Users.GetAll().Include(_ => _.Role).OrderBy(_ => _.UserId);

            PaginatedResponse<User> paginatedResponse = await PaginatedResponse<User>.CreateAsync(users, pageNumber, pageSize);

            PaginatedResponse<UserResponseDTO> mappedResponse = _mapper.Map<PaginatedResponse<UserResponseDTO>>(paginatedResponse);

            //_response.IsSuccess = true;
            //_response.StatusCode = HttpStatusCode.OK;
            //_response.Result = mappedResponse;

            return Ok(APIResponse.Success(HttpStatusCode.OK, mappedResponse));
        }


        [RoleAuthorize(ApiRole.Admin)]
        [HttpPut("UpdateUserDetails")]
        public ActionResult UpdateUserDetails([FromBody] UserUpdateDTO userUpdate)
        {
            User user = _mapper.Map<User>(userUpdate);

            APIResponse response = _userService.UpdateUserDetail(user);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("DeleteUser/{userId:int}")]
        public ActionResult DeleteUser(int userId)
        {

            APIResponse response = _userService.DeleteUser(userId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return StatusCode((int)response.StatusCode, response);

        }

        [RoleAuthorize(ApiRole.Admin)]
        [HttpGet("GetDeletedUsers")]
        public async Task<ActionResult<APIResponse>> GetDeletedUsers(int pageNumber = 1, int pageSize = 20)
        {
            IQueryable<User> users = _unitOfWork.Users.GetDeleteUsers().Include(_ => _.Role).OrderBy(_ => _.UserId);

            PaginatedResponse<User> paginatedResponse = await PaginatedResponse<User>.CreateAsync(users, pageNumber, pageSize);

            PaginatedResponse<UserResponseDTO> mappedResponse = _mapper.Map<PaginatedResponse<UserResponseDTO>>(paginatedResponse);

            //_response.IsSuccess = true;
            //_response.StatusCode = HttpStatusCode.OK;
            //_response.Result = mappedResponse;

            return Ok(APIResponse.Success(HttpStatusCode.OK, mappedResponse));
        }

        [HttpPut("RestoreUser/{userId:int}")]
        public ActionResult RestoreUser(int userId)
        {

            APIResponse response = _userService.RestoreUser(userId);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return StatusCode((int)response.StatusCode, response);

        }

    }
}
