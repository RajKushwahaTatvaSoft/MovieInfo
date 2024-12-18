using MovieInfoWebAPI.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Repositories.IRepository
{
    public interface IMovieDetailRepository : IGenericRepository<MovieDetail>
    {
        public IQueryable<MovieDetail> GetDeleteMovies();
        public MovieDetail? GetDeletedMovie(Expression<Func<MovieDetail, bool>> filter);
    }
}
