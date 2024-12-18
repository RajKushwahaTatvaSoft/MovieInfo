using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Services.Constants
{
    public class APIMessages
    {
        public const string MOVIE_NOT_FOUND = "Movie Not Found";
        public const string MOVIE_ALREADY_EXISTS = "Movie already exists with given ";
        public const string MOVIE_UPDATE_SUCCESS= "Movie Updated Successfully";


        public const string CREW_ADDED_SUCCESS= "Crew Added Successfully";

        public const string USER_NOT_FOUND = "User Not Found";

        public const string PERSON_NOT_FOUND = "Person Not Found";

        public const string REVIEW_ALREADY_EXISTS= "User already has review submitted. Could not add another";
        public const string REVIEW_ADDED_SUCCESS= "Review Added Successfully";
        public const string REVIEW_NOT_FOUND= "Review Not Found";
        public const string REVIEW_UPDATE_SUCCESS= "Review Updated Successfully";
    }
}
