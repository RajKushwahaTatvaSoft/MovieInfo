using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Services.Utilities
{
    public static class FileHelper
    {

        public static async Task InsertFileAfterRename(IFormFile file, string path, string updateName)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string[] oldfiles = Directory.GetFiles(path, updateName + ".*");
            foreach (string f in oldfiles)
            {
                File.Delete(f);
            }

            string extension = Path.GetExtension(file.FileName);

            string fileName = updateName + extension;

            string fullPath = Path.Combine(path, fileName);

            using FileStream stream = new(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        public static void InsertFileAtPath(IFormFile document, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = document.FileName;
            string fullPath = Path.Combine(path, fileName);

            using FileStream stream = new(fullPath, FileMode.Create);
            document.CopyTo(stream);
        }

        public static async Task InsertUserProfilePhoto(IFormFile profilePhoto, string webRootPath, int userId)
        {
            string profilePath = Path.Combine(webRootPath, "ServerData", "User", $"{userId}");

            await InsertFileAfterRename(profilePhoto, profilePath, "ProfilePhoto");
        }
    }
}
