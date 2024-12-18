using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Models.DTO.UserDTO
{
    public class ProfileUpdateDTO
    {
        public int UserId { get; set; }
        public IFormFile? ProfilePhotoFile { get; set; }
    }
}
