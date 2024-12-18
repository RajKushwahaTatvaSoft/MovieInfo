using MovieInfoWebAPI.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Repositories.IRepository
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public IQueryable<User> GetDeleteUsers();
        public User? GetDeletedUser(Expression<Func<User, bool>> filter);
    }
}
