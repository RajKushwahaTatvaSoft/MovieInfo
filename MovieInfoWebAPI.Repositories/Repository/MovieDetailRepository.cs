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
    internal class MovieDetailRepository : GenericRepository<MovieDetail>, IMovieDetailRepository
    {
        private ApplicationDbContext _context;
        public MovieDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public override MovieDetail? GetFirstOrDefault(Expression<Func<MovieDetail, bool>> filter)
        {
            IQueryable<MovieDetail> query = dbSet.Where(movie => !movie.IsDeleted);
            return query.FirstOrDefault(filter);
        }

        public override IQueryable<MovieDetail> GetAll()
        {
            IQueryable<MovieDetail> query = dbSet.Where(movie => !movie.IsDeleted);
            return query;
        }

        public override IQueryable<MovieDetail> Where(Expression<Func<MovieDetail, bool>> filter)
        {
            IQueryable<MovieDetail> query = dbSet.Where(movie => !movie.IsDeleted);
            return query.Where(filter);
        }
        public override bool Any(Expression<Func<MovieDetail, bool>> filter)
        {
            IQueryable<MovieDetail> query = dbSet.Where(movie => !movie.IsDeleted);
            return query.Any(filter);
        }


        public IQueryable<MovieDetail> GetDeleteMovies()
        {
            IQueryable<MovieDetail> query = dbSet.Where(movie => movie.IsDeleted);
            return query;
        }

        public MovieDetail? GetDeletedMovie(Expression<Func<MovieDetail, bool>> filter)
        {
            IQueryable<MovieDetail> query = dbSet.Where(movie => movie.IsDeleted);
            return query.FirstOrDefault(filter);
        }
    }
}
