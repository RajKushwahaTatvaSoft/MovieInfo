using System.Net;

namespace MovieInfoWebAPI.Models.CustomModels
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
        }

        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; }
        public object? Result { get; set; }


        public static APIResponse Success(HttpStatusCode statusCode, object? resultData = null)
        {
            APIResponse response = new APIResponse();

            response.StatusCode = statusCode;
            response.IsSuccess = true;
            response.Result = resultData;

            return response;
        }
        public static APIResponse Created(object? resultData = null)
        {
            APIResponse response = new APIResponse();

            response.StatusCode = HttpStatusCode.Created;
            response.IsSuccess = true;
            response.Result = resultData;

            return response;
        }

        public static APIResponse OK(object? resultData = null)
        {
            APIResponse response = new APIResponse();

            response.StatusCode = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Result = resultData;

            return response;
        }

        public static APIResponse NotFound(params string[] errorMessages)
        {
            APIResponse response = new APIResponse();

            response.StatusCode = HttpStatusCode.NotFound;
            response.IsSuccess = false;
            response.ErrorMessages = errorMessages.ToList();

            return response;
        }

        public static APIResponse InternalServerError(params string[] errorMessages)
        {
            APIResponse response = new APIResponse();

            response.StatusCode = HttpStatusCode.InternalServerError;
            response.IsSuccess = false;
            response.ErrorMessages = errorMessages.ToList();

            return response;
        }

        public static APIResponse Error(HttpStatusCode statusCode, params string[] errorMessages)
        {
            APIResponse response = new APIResponse();

            response.StatusCode = statusCode;
            response.IsSuccess = false;
            response.ErrorMessages = errorMessages.ToList();

            return response;
        }
    }
}