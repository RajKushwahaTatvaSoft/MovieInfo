using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Services.IServices
{
    public interface IUserService
    {
        public APIResponse UpdateUserDetail(User userUpdate);
        public APIResponse DeleteUser(int userId);
        public APIResponse RestoreUser(int userId);
    }
}
