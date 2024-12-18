using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.CustomTableModels;
using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Models.DTO.PersonDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Repositories.IRepository
{
    public interface ICastRepository : IGenericRepository<Cast>
    {
        public Task<PaginatedResponse<CastItem>> GetCastItems(string searchActorName, int pageNum, int pageSize);
        public Task<IEnumerable<PersonSearchItemDTO>> GetCastSuggestions(string searchActorName);

    }
}
