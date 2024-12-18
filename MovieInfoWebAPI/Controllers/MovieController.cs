using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieInfoWebAPI.Authorization;
using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.DataContext;
using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Models.DTO.AdminDTO;
using MovieInfoWebAPI.Models.DTO.MovieDTO;
using MovieInfoWebAPI.Repositories.IRepository;
using MovieInfoWebAPI.Services.Constants;
using MovieInfoWebAPI.Services.IServices;
using MovieInfoWebAPI.Services.Services.Helper.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace MovieInfoWebAPI.Controllers
{
    [Route("MovieInfo/Movie")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMapper _mapper;
        protected APIResponse _response;
        private readonly IMovieService _movieService;
        private readonly IUnitOfWork _unitOfWork;

        public MovieController(IMapper mapper, IMovieService movieService, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _response = new();
            _movieService = movieService;
            _unitOfWork = unitOfWork;
        }

        [RoleAuthorize(ApiRole.Admin)]
        [HttpGet("GetMoviesReleasedByYear")]
        public ActionResult<APIResponse> GetMoviesReleasedByYear(int pageNumber = 1, int pageSize = 10, bool isOrderByRecent = true)
        {

            try
            {
                IEnumerable<MovieReleaseYearDTO> result = _unitOfWork.MovieDetails.GetAll()
                    .Where(m => m.ReleaseDate != null)
                    .GroupBy(m => m.ReleaseDate.HasValue ? m.ReleaseDate.Value.Year : 0)
                    .Select(g => new MovieReleaseYearDTO
                    {
                        ReleaseYear = g.Key,
                        MovieCount = g.Count()
                    })
                    .OrderBy(dto => dto.ReleaseYear);
                //.Skip((pageNumber-1)*pageSize).Take(pageSize);

                return Ok(APIResponse.OK(result));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [RoleAuthorize(ApiRole.Admin)]
        [HttpGet("GetAvgBudgetAndRevenueYear")]
        public ActionResult<APIResponse> GetAvgBudgetAndRevenueYear(int pageNumber = 1, int pageSize = 10, bool isOrderByRecent = true)
        {

            try
            {
                IEnumerable<BudgetAndRevenueDTO> result = _unitOfWork.MovieDetails.GetAll()
                    .Where(m => m.ReleaseDate != null && m.Budget > 0 && m.Revenue > 0)
                    .GroupBy(m => m.ReleaseDate.HasValue ? m.ReleaseDate.Value.Year : 0)
                    .Select(g => new BudgetAndRevenueDTO
                    {
                        ReleaseYear = g.Key,
                        AvgBudget = g.Average(m => m.Budget),
                        AvgRevenue = g.Average(m => m.Revenue)
                    })
                    .OrderBy(dto => dto.ReleaseYear);
                //.Skip((pageNumber-1)*pageSize).Take(pageSize);

                return Ok(APIResponse.OK(result));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [RoleAuthorize(ApiRole.Admin)]
        [HttpPost("AddMovie")]
        public ActionResult<APIResponse> AddNewMovie([FromBody] MovieCreateDTO movieCreate)
        {
            try
            {
                MovieDetail movieDetail = _mapper.Map<MovieDetail>(movieCreate);
                APIResponse serviceResponse = _movieService.AddNewMovie(movieDetail);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(serviceResponse);
                }

                return StatusCode((int)serviceResponse.StatusCode, serviceResponse);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [RoleAuthorize(ApiRole.Admin)]
        [HttpPut("UpdateMovie")]
        public ActionResult<APIResponse> UpdateMovieById([FromBody] MovieUpdateDTO movieUpdate)
        {

            try
            {

                MovieDetail movieDetail = new MovieDetail();

                movieDetail.MovieId = movieUpdate.MovieId;
                movieDetail.Actors = movieUpdate.Actor;
                movieDetail.Writer = movieUpdate.Writer;
                movieDetail.Director = movieUpdate.Director;
                movieDetail.Title = movieUpdate.Title;
                movieDetail.ReleaseDate = movieUpdate.ReleaseDate;
                movieDetail.Revenue = movieUpdate.Revenue ?? movieDetail.Revenue;
                movieDetail.Budget = movieUpdate.Budget ?? movieDetail.Budget;
                movieDetail.Duration = movieUpdate.Duration;
                movieDetail.PosterUrl = movieUpdate.PosterUrl;

                APIResponse serviceResponse = _movieService.UpdateMovie(movieDetail);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(serviceResponse);
                }

                return StatusCode((int)serviceResponse.StatusCode, serviceResponse);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [RoleAuthorize(ApiRole.Admin)]
        [HttpDelete("DeleteMovie/{movieId:int}")]
        public ActionResult<APIResponse> DeleteMovieWithId(int movieId)
        {
            try
            {

                APIResponse serviceResponse = _movieService.DeleteMovie(movieId);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(serviceResponse);
                }

                return StatusCode((int)serviceResponse.StatusCode, serviceResponse);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [RoleAuthorize(ApiRole.Admin)]
        [HttpDelete("RestoreMovie/{movieId:int}")]
        public ActionResult<APIResponse> RestoreMovieWithId(int movieId)
        {
            try
            {

                APIResponse serviceResponse = _movieService.RestoreMovie(movieId);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(serviceResponse);
                }

                return StatusCode((int)serviceResponse.StatusCode, serviceResponse);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("GetMovies")]
        public async Task<ActionResult<APIResponse>> GetMovies(string searchName = "", int category = 0, int pageNumber = 1, int pageSize = 10)
        {
            try
            {

                APIResponse serviceResponse = await _movieService.GetMovies(searchName, category, pageNumber, pageSize);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(serviceResponse);
                }

                return StatusCode((int)serviceResponse.StatusCode, serviceResponse);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetDeletedMovies")]
        public async Task<ActionResult<APIResponse>> GetDeletedMovies(string searchName = "", int category = 0, int pageNumber = 1, int pageSize = 10)
        {
            try
            {

                APIResponse serviceResponse = await _movieService.GetDeletedMovies(searchName, category, pageNumber, pageSize);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(serviceResponse);
                }

                return StatusCode((int)serviceResponse.StatusCode, serviceResponse);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [RoleAuthorize(ApiRole.AllRoles)]
        [HttpGet("GetSuggestionForMovieSearch")]
        public ActionResult<APIResponse> GetSuggestionForMovieSearch(string searchInput)
        {
            try
            {
                APIResponse serviceResponse = _movieService.GetSuggestionForMovieSearch(searchInput);

                if (serviceResponse.IsSuccess)
                {
                    return Ok(serviceResponse);
                }

                return StatusCode((int)serviceResponse.StatusCode, serviceResponse);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
                return _response;
            }
        }

        [RoleAuthorize(ApiRole.AllRoles)]
        [HttpGet("GetGenres")]
        public ActionResult<APIResponse> GetGenres()
        {
            try
            {
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = _movieService.GetMovieGenres();
                return _response;
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
                return _response;
            }
        }


        [RoleAuthorize(ApiRole.AllRoles)]
        [HttpGet("GetMovieByName")]
        public async Task<ActionResult<APIResponse>> GetMovieByName(string movieName = "", int pageSize = 12, int pageNumber = 1)
        {

            try
            {
                PaginatedResponse<MovieDetail> pagedList = await _movieService.GetMovieByName(movieName, pageSize, pageNumber);

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = pagedList;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
                return _response;
            }
        }

        [RoleAuthorize(ApiRole.AllRoles)]
        [HttpGet("GetMovieByCategory")]
        public async Task<ActionResult<APIResponse>> GetCategoryMovie(string category, int pageSize = 12, int pageNumber = 1)
        {

            try
            {
                PaginatedResponse<MovieDetail> pagedList = await _movieService.GetCategoryMovies(category, pageSize, pageNumber);

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = pagedList;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
                return _response;
            }
        }

        [RoleAuthorize(ApiRole.AllRoles)]
        [HttpGet("GetMovieById/{movieId:int}")]
        public ActionResult<APIResponse> GetMovieDetailById(int movieId)
        {
            try
            {

                MovieDetail? movie = _movieService.GetMovieDetailById(movieId);

                if (movie == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages.Add(APIMessages.MOVIE_NOT_FOUND);
                    return NotFound();
                }

                MovieResponseDTO movieDTO = _mapper.Map<MovieResponseDTO>(movie);
                foreach (var genreMapping in movie.MovieGenreMappings)
                {
                    movieDTO.GenreList.Add(genreMapping.Genre.GenreName);
                }

                foreach (var cast in movie.Casts)
                {
                    string castName = cast.Person.PersonName;
                    movieDTO.CastList.Add(castName);
                }

                foreach (var crew in movie.Crews)
                {
                    string crewName = crew.Person.PersonName;
                    movieDTO.CrewList.Add(crewName);
                }

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = movieDTO;
                return Ok(_response);

            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, _response);

            }
        }



        #region Helper Functions For Setting Up DB


        //[HttpGet("UpdateJobAndDepartment")]
        //private ActionResult UpdateJobDepartmentFromMovieCredit(int page = 1)
        //{
        //    try
        //    {
        //        int pageSize = 10000;
        //        List<MovieDetail> movieLists = _context.MovieDetails.OrderBy(_ => _.MovieId).Skip((page - 1) * pageSize).Take(40000).AsNoTracking().ToList();
        //        int creditHit = 0;

        //        List<string> DepartmentList = _context.Departments.Select(_ => _.DepartmentName).ToList();
        //        List<Job> JobTitleList = _context.Jobs.Include(_ => _.Department).ToList();

        //        List<string> addDepartmentList = new List<string>();
        //        List<Dictionary<string, string>> addJobList = new List<Dictionary<string, string>>();
        //        int moreThanOneCreditHit = 0;
        //        //foreach (var movie in movieLists)
        //        //{
        //        //    IEnumerable<string?>? moviecredit = _context.Moviecredits.AsNoTracking().Where(credit => credit.Imdbid == movie.ImdbId)?.Select(_ => _.Crew);
        //        //    if (moviecredit == null || moviecredit.Count() == 0)
        //        //    {
        //        //        continue;
        //        //    }

        //        //    creditHit++;

        //        //    if (moviecredit.Count() > 1)
        //        //    {
        //        //        moreThanOneCreditHit++;
        //        //        continue;
        //        //    }


        //        //    string crew = moviecredit.FirstOrDefault()?.Replace("None", "''") ?? "";
        //        //    //Tempgenre? genretable = _context.Tempgenres.FirstOrDefault(genre => genre.Imdbid == movie.ImdbId);
        //        //    //if (genretable == null)
        //        //    //{
        //        //    //    continue;
        //        //    //}


        //        //    //dynamic? parsedJson =JsonConvert.DeserializeObject(moviecredit.Crew ?? "");
        //        //    //string jsonstring= JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        //        //    JArray jsonobject = JArray.Parse(crew); //was when genre was present as json in database
        //        //    //JArray jsonobject = JArray.Parse("");

        //        //    foreach (var json in jsonobject)
        //        //    {
        //        //        string? department = json.Value<string>("department");
        //        //        string? job = json.Value<string>("job");
        //        //        if (department == null || job == null)
        //        //        {
        //        //            continue;
        //        //        }

        //        //        if (!DepartmentList.Contains(department))
        //        //        {
        //        //            DepartmentList.Add(department);
        //        //            addDepartmentList.Add(department);
        //        //        }

        //        //        if (!JobTitleList.Any(dbJob => dbJob.JobTitle.ToLower().Equals(job.ToLower())))
        //        //        {
        //        //            Department dept = new Department()
        //        //            {
        //        //                DepartmentName = department,
        //        //                DepartmentId = 0,
        //        //            };

        //        //            Job addJob = new Job()
        //        //            {
        //        //                JobTitle = job,
        //        //                Department = dept,
        //        //            };
        //        //            JobTitleList.Add(addJob);
        //        //            Dictionary<string, string> dict = new Dictionary<string, string>()
        //        //            {
        //        //                { "job" , job },
        //        //                { "department", department }
        //        //            };
        //        //            addJobList.Add(dict);
        //        //        }

        //        //        //int genreId = genreLists.FirstOrDefault(genre => genre.GenreName.Equals(genreName))?.GenreId ?? 0;
        //        //        //MovieGenreMapping mapping = new MovieGenreMapping()
        //        //        //{
        //        //        //    MovieId = movie.MovieId,
        //        //        //    GenreId = genreId
        //        //        //};

        //        //        //_context.MovieGenreMappings.Add(mapping);
        //        //    }

        //        //}

        //        int i = 0;

        //        foreach (var departmentName in addDepartmentList)
        //        {
        //            Department department = new Department()
        //            {
        //                DepartmentName = departmentName,
        //            };
        //            _context.Departments.Add(department);
        //        }

        //        _context.SaveChanges();


        //        List<Department> updatedDepartments = _context.Departments.AsNoTracking().ToList();
        //        int noDeptHit = 0;
        //        foreach (var jobDict in addJobList)
        //        {
        //            string job = jobDict["job"];
        //            string departmentName = jobDict["department"];

        //            int deptId = updatedDepartments.FirstOrDefault(dept => dept.DepartmentName.ToLower().Equals(departmentName.ToLower()))?.DepartmentId ?? 0;

        //            if (deptId == 0)
        //            {
        //                noDeptHit++;
        //                continue;
        //            }

        //            Job addJob = new Job()
        //            {
        //                JobTitle = job,
        //                DepartmentId = deptId,
        //            };
        //            _context.Jobs.Add(addJob);
        //        }

        //        _context.SaveChanges();

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}


        //[RoleAuthorize(ApiRole.Admin)]
        //[HttpGet("UpdateMoviesRating")]
        //private ActionResult UpdateAllMoviesRating(int page = 1)
        //{
        //    try
        //    {
        //        int pageSize = 10000;
        //        IEnumerable<MovieDetail> movieList = _context.MovieDetails.Include(_ => _.UserRatings).Skip((page - 1) * pageSize).Take(pageSize);

        //        foreach (var movie in movieList)
        //        {
        //            IEnumerable<UserRating> userRatings = movie.UserRatings;
        //            int ratingCount = userRatings.Count();
        //            decimal totalRating = 0;
        //            decimal ratingAverage = 0;
        //            if (userRatings.Count() != 0)
        //            {

        //                foreach (var rating in userRatings)
        //                {
        //                    totalRating += rating.Rating;
        //                }

        //                ratingAverage = Math.Round(totalRating / (decimal)ratingCount, 2);
        //            }

        //            movie.RatingCount = ratingCount;
        //            movie.Rating = ratingAverage;
        //            _context.Update(movie);
        //        }

        //        _context.SaveChanges();

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}


        //[RoleAuthorize(ApiRole.Admin)]
        //[HttpGet("UpdateGenresFromMovies")]
        //private ActionResult UpdateGenresFromMovies(int page = 1)
        //{
        //    try
        //    {
        //        int pageSize = 10000;
        //        IEnumerable<MovieGenre> genreLists = _context.MovieGenres.ToList();
        //        IEnumerable<MovieDetail> movieLists = _context.MovieDetails.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        //        foreach (var movie in movieLists)
        //        {
        //            //Tempgenre? genretable = _context.Tempgenres.FirstOrDefault(genre => genre.Imdbid == movie.ImdbId);
        //            //if (genretable == null)
        //            //{
        //            //    continue;
        //            //}

        //            //JArray jsonobject = JArray.Parse(movie.Genres); was when genre was present as json in database
        //            JArray jsonobject = JArray.Parse("");

        //            foreach (var json in jsonobject)
        //            {
        //                string? genreName = json.Value<string>("name");
        //                if (genreName == null)
        //                {
        //                    continue;
        //                }

        //                int genreId = genreLists.FirstOrDefault(genre => genre.GenreName.Equals(genreName))?.GenreId ?? 0;
        //                MovieGenreMapping mapping = new MovieGenreMapping()
        //                {
        //                    MovieId = movie.MovieId,
        //                    GenreId = genreId
        //                };

        //                _context.MovieGenreMappings.Add(mapping);
        //            }

        //        }
        //        _context.SaveChanges();

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpGet("UpdateCast")]
        //private ActionResult UpdateCastFromMovieCredit()
        //{
        //    int movieId = 0;
        //    bool isCrew = false;
        //    try
        //    {

        //        List<string> doubleCredits = new List<string>();
        //        List<string> noCrewJob = new List<string>();
        //        List<Job> jobsList = _context.Jobs.Include(_ => _.Department).AsNoTracking().ToList();
        //        List<Person> personList = _context.People.AsNoTracking().ToList();
        //        IEnumerable<MovieDetail> movieLists = _context.MovieDetails.OrderBy(_ => _.MovieId).Skip(33325).Take(40000).AsNoTracking().ToList();

        //        //IEnumerable<Moviecredit> moviecreditList = _context.Moviecredits.AsNoTracking().ToList();

        //        //foreach (var movie in movieLists)
        //        //{
        //        //    movieId = movie.MovieId;

        //        //    Console.WriteLine(movieId);

        //        //    IEnumerable<Moviecredit>? moviecredits = moviecreditList.Where(credit => credit.Imdbid == movie.ImdbId);
        //        //    if (moviecredits == null || moviecredits.Count() == 0)
        //        //    {
        //        //        continue;
        //        //    }

        //        //    if (moviecredits.Count() > 1)
        //        //    {
        //        //        doubleCredits.Add(movie.ImdbId);
        //        //        continue;
        //        //    }

        //        //    Moviecredit? movieCredit = moviecredits.FirstOrDefault();

        //        //    string crew = movieCredit?.Crew?.Replace("None", "''") ?? "";
        //        //    string cast = movieCredit?.Cast?.Replace("None", "''") ?? "";
        //        //    cast = cast.Replace("('')", "");
        //        //    JArray crewJson = JArray.Parse(crew); //was when genre was present as json in database
        //        //    JArray castJson = JArray.Parse(cast); //was when genre was present as json in database

        //        //    foreach (var json in crewJson)
        //        //    {

        //        //        isCrew = true;
        //        //        string? department = json.Value<string>("department");
        //        //        string? job = json.Value<string>("job");
        //        //        string? name = json.Value<string>("name");
        //        //        string? profilePath = json.Value<string>("profile_path");
        //        //        int? gender = json.Value<int>("gender");

        //        //        if (department == null || job == null || name == null || profilePath == null || gender == null)
        //        //        {
        //        //            continue;
        //        //        }

        //        //        Job? crewJob = jobsList.FirstOrDefault(dbJob => dbJob.JobTitle == job && dbJob.Department.DepartmentName == department);

        //        //        if (crewJob == null)
        //        //        {
        //        //            Department? dbDepart = _context.Departments.FirstOrDefault(dept => dept.DepartmentName == department);

        //        //            if (dbDepart == null)
        //        //            {
        //        //                dbDepart = new Department() { DepartmentName = department };
        //        //                _context.Departments.Add(dbDepart);
        //        //                _context.SaveChanges();
        //        //            }

        //        //            Job dbJob = new Job()
        //        //            {
        //        //                JobTitle = job,
        //        //                DepartmentId = dbDepart.DepartmentId,
        //        //            };

        //        //            _context.Jobs.Add(dbJob);
        //        //            _context.SaveChanges();

        //        //            jobsList.Add(dbJob);
        //        //            crewJob = dbJob;
        //        //        }

        //        //        Person? person = personList.FirstOrDefault(person => person.PersonName == name);

        //        //        if (person == null)
        //        //        {

        //        //            person = new Person()
        //        //            {
        //        //                PersonName = name,
        //        //                ProfilePath = profilePath,
        //        //                Gender = gender,
        //        //            };

        //        //            _context.People.Add(person);
        //        //            _context.SaveChanges();

        //        //            personList.Add(person);
        //        //        }


        //        //        Crew addCrew = new Crew()
        //        //        {
        //        //            JobId = crewJob.JobId,
        //        //            MovieId = movie.MovieId,
        //        //            PersonId = person.PersonId
        //        //        };

        //        //        _context.Crews.Add(addCrew);
        //        //        _context.SaveChanges();

        //        //    }

        //        //    foreach (var json in castJson)
        //        //    {

        //        //        isCrew = false;
        //        //        string? character = json.Value<string>("character");
        //        //        int? order = json.Value<int>("order");
        //        //        string? name = json.Value<string>("name");
        //        //        string? profilePath = json.Value<string>("profile_path");
        //        //        int? gender = json.Value<int>("gender");

        //        //        if (character == null || order == null || name == null || profilePath == null || gender == null)
        //        //        {
        //        //            continue;
        //        //        }

        //        //        Person? person = personList.FirstOrDefault(person => person.PersonName == name);

        //        //        if (person == null)
        //        //        {

        //        //            person = new Person()
        //        //            {
        //        //                PersonName = name,
        //        //                ProfilePath = profilePath,
        //        //                Gender = gender,
        //        //            };

        //        //            _context.People.Add(person);
        //        //            _context.SaveChanges();

        //        //            personList.Add(person);
        //        //        }

        //        //        Cast addCast = new Cast()
        //        //        {
        //        //            MovieId = movie.MovieId,
        //        //            PersonId = person.PersonId,
        //        //            CastOrder = order,
        //        //            Character = character,
        //        //        };

        //        //        _context.Casts.Add(addCast);
        //        //        _context.SaveChanges();

        //        //    }
        //        //}

        //        Console.WriteLine(DateTime.Now.ToString("f"));
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(DateTime.Now.ToString("f"));
        //        return BadRequest(ex.Message + movieId + " " + isCrew);
        //    }

        //}

        [RoleAuthorize(ApiRole.Admin)]
        [HttpGet("GetDetailFromOMDB")]
        public async Task<ActionResult<APIResponse>> FetchMovieDetailsFromOMDB(string imdbId)
        {

            var apiKey = "c969315c"; // Replace with your actual key

            HttpClient httpClient = new HttpClient();
            try
            {

                var url = $"http://www.omdbapi.com/?apikey={apiKey}&i={imdbId}";
                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound(APIResponse.NotFound("Couldn't fetch from omdb " + imdbId));
                }

                var content = await response.Content.ReadAsStringAsync();
                OMDBMovieDataResponse? omdbMovie = System.Text.Json.JsonSerializer.Deserialize<OMDBMovieDataResponse>(content);

                if (omdbMovie == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, APIResponse.InternalServerError("Couldn't convert " + imdbId));
                }

                return Ok(APIResponse.OK(omdbMovie));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal Server Error: {ex.Message}");
            }
        }

        //[RoleAuthorize(ApiRole.User)]
        //[HttpPost("UpdateMovieDataFromOMDB")]
        //public async Task<ActionResult> UpdateMovieDataFromOMDB([FromBody] IEnumerable<string> movieImdbIds)
        //{

        //    var apiKey = "c969315c"; // Replace with your actual key

        //    HttpClient httpClient = new HttpClient();
        //    try
        //    {
        //        movieImdbIds = movieImdbIds.Distinct();
        //        foreach (string movieId in movieImdbIds)
        //        {
        //            MovieDetail? movieData = _context.MovieDetails.FirstOrDefault(movie => movie.ImdbId != null && movie.ImdbId.ToLower().Equals(movieId.ToLower()));

        //            if (movieData == null)
        //            {
        //                Console.WriteLine("Couldn't fetch from database " + movieId);
        //                continue;
        //            }

        //            var url = $"http://www.omdbapi.com/?apikey={apiKey}&i={movieId}";
        //            var response = await httpClient.GetAsync(url);

        //            if (!response.IsSuccessStatusCode)
        //            {
        //                Console.WriteLine("Couldn't fetch from omdb " + movieId);
        //                continue;
        //            }

        //            var content = await response.Content.ReadAsStringAsync();
        //            OMDBMovieDataResponse? omdbMovie = System.Text.Json.JsonSerializer.Deserialize<OMDBMovieDataResponse>(content);

        //            if (omdbMovie == null)
        //            {
        //                Console.WriteLine("Couldn't convert " + movieId);
        //                continue;
        //            }

        //            movieData.PosterUrl = omdbMovie.Poster ?? movieData.PosterUrl;
        //            _context.MovieDetails.Update(movieData);
        //            _context.SaveChanges();
        //        }

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal Server Error: {ex.Message}");
        //    }

        //}


        #endregion

    }
}
