using MovieInfoWebAPI.Models.DataContext;
using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Repositories.Repository
{
    internal class UserRepository : GenericRepository<User>, IUserRepository
    {
        private ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public override User? GetFirstOrDefault(Expression<Func<User, bool>> filter)
        {
            IQueryable<User> query = dbSet.Where(user => !user.IsDeleted);
            return query.FirstOrDefault(filter);
        }

        public override IQueryable<User> GetAll()
        {
            IQueryable<User> query = dbSet.Where(user => !user.IsDeleted);
            return query;
        }

        public override IQueryable<User> Where(Expression<Func<User, bool>> filter)
        {
            IQueryable<User> query = dbSet.Where(user => !user.IsDeleted);
            return query.Where(filter);
        }
        public override bool Any(Expression<Func<User, bool>> filter)
        {
            IQueryable<User> query = dbSet.Where(user => !user.IsDeleted);
            return query.Any(filter);
        }

        public IQueryable<User> GetDeleteUsers()
        {
            IQueryable<User> query = dbSet.Where(user => user.IsDeleted);
            return query;
        }

        public User? GetDeletedUser(Expression<Func<User, bool>> filter)
        {
            IQueryable<User> query = dbSet.Where(user => user.IsDeleted);
            return query.FirstOrDefault(filter);
        }
    }
}
