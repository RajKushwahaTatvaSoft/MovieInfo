using Microsoft.Extensions.Configuration;
using MovieInfoWebAPI.Models.DataContext;
using MovieInfoWebAPI.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Repositories.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        private ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context,IConfiguration _config)
        {
            _context = context;
            MovieDetails = new MovieDetailRepository(_context);
            MovieGenres = new MovieGenreRepository(_context);
            MovieGenreMaps = new MovieGenreMapRepository(_context);
            Users = new UserRepository(_context);
            UserRatings = new UserRatingRepository(_context);
            Casts = new CastRepository(_context,_config);
            Crews = new CrewRepository(_context);
            Jobs = new JobRepository(_context);
            People = new PersonRepository(_context);
            Departments = new DepartmentRepository(_context);
            Roles = new RoleRepository(_context);
        }

        public IMovieDetailRepository MovieDetails { get; private set; }
        public IMovieGenreRepository MovieGenres { get; private set; }
        public IMovieGenreMapRepository MovieGenreMaps { get; private set; }
        public IUserRepository Users { get; private set; }
        public IUserRatingRepository UserRatings { get; private set; }
        public IRoleRepository Roles { get; private set; }
        public IPersonRepository People { get; private set; }
        public ICastRepository Casts { get; private set; }
        public ICrewRepository Crews { get; private set; }
        public IJobRepository Jobs { get; private set; }
        public IDepartmentRepository Departments { get; private set; }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
