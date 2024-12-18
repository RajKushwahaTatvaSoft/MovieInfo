using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Repositories.IRepository;
using MovieInfoWebAPI.Services.Constants;
using MovieInfoWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unit)
        {
            _unitOfWork = unit;
        }

        public APIResponse UpdateUserDetail(User userUpdate)
        {
            User? userDb = _unitOfWork.Users.GetFirstOrDefault(user => user.UserId == userUpdate.UserId);

            if (userDb == null)
            {
                return APIResponse.NotFound(APIMessages.USER_NOT_FOUND);
            }

            userDb.Email = userUpdate.Email;
            userDb.FirstName = userUpdate.FirstName;
            userDb.LastName = userUpdate.LastName;
            userDb.RoleId = userUpdate.RoleId;

            _unitOfWork.Users.Update(userDb);
            _unitOfWork.Save();

            return APIResponse.Success(HttpStatusCode.OK, userDb.UserId);

        }

        public APIResponse DeleteUser(int userId)
        {
            User? userDb = _unitOfWork.Users.GetFirstOrDefault(user => user.UserId == userId);

            if (userDb == null)
            {
                return APIResponse.NotFound(APIMessages.USER_NOT_FOUND);
            }

            userDb.IsDeleted = true;
            userDb.DeletedDate = DateTime.Now;
            _unitOfWork.Users.Update(userDb);
            _unitOfWork.Save();

            return APIResponse.Success(HttpStatusCode.OK, userDb.UserId);

        }

        public APIResponse RestoreUser(int userId)
        {
            User? userDb = _unitOfWork.Users.GetDeletedUser(user => user.UserId == userId);

            if (userDb == null)
            {
                return APIResponse.NotFound(APIMessages.USER_NOT_FOUND);
            }

            userDb.IsDeleted = false;
            userDb.DeletedDate = null;
            _unitOfWork.Users.Update(userDb);
            _unitOfWork.Save();

            return APIResponse.Success(HttpStatusCode.OK, userDb.UserId);

        }
    }
}
