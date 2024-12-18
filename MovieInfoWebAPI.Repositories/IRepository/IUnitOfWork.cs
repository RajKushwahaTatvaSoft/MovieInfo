using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Repositories.IRepository
{
    public interface IUnitOfWork
    {
        IMovieDetailRepository MovieDetails { get; }
        IMovieGenreRepository MovieGenres { get; }
        IMovieGenreMapRepository MovieGenreMaps { get; }
        IUserRepository Users { get; }
        IUserRatingRepository UserRatings { get; }
        ICastRepository Casts { get; }
        ICrewRepository Crews { get; }
        IJobRepository Jobs { get; }
        IDepartmentRepository Departments { get; }
        IPersonRepository People { get; }
        IRoleRepository Roles { get; }
        void Save();
    }
}
