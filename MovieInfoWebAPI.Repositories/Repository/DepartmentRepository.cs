using MovieInfoWebAPI.Models.DataContext;
using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Repositories.Repository
{
    internal class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        private ApplicationDbContext _context;
        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
