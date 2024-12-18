using MovieInfoWebAPI.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace MovieInfoWebAPI.Services.Utilities
{
    public static class ImageHelper
    {
        public const string UI_AVATAR_URL = "https://ui-avatars.com/api/?background=random&name=";
        public const string USER_DATA_URL = "https://localhost:7048/ServerData/User";
        public static string GetUIAvatarURL(string? firstName, string? lastName)
        {
            return $"{UI_AVATAR_URL}{firstName} {lastName}";
        }

        public static string GetUserProfileURL(string userProfileName, int userId)
        {
            return $"{USER_DATA_URL}/{userId}/{userProfileName}";
        }

        public static string GetProfileURLFromUser(User user)
        {
            if (user.ProfilePhotoName == null)
            {
                return GetUIAvatarURL(user.FirstName,user.LastName);
            }

            return GetUserProfileURL(user.ProfilePhotoName,user.UserId);
        }

    }
}
