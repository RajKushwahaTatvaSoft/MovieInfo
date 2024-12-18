using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Models.CustomTableModels;
using MovieInfoWebAPI.Models.DataContext;
using MovieInfoWebAPI.Models.DataModels;
using MovieInfoWebAPI.Models.DTO.PersonDTO;
using MovieInfoWebAPI.Repositories.IRepository;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Repositories.Repository
{
    internal class CastRepository : GenericRepository<Cast>, ICastRepository
    {
        private ApplicationDbContext _context;
        private readonly string _connectionString;
        public CastRepository(ApplicationDbContext context, IConfiguration configuration) : base(context)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public async Task<PaginatedResponse<CastItem>> GetCastItems(string searchActorName, int pageNum = 1, int pageSize = 12)
        {
                   
            int totalCount = 0;
            searchActorName = searchActorName.Trim();
            List<CastItem> results = new List<CastItem>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var countCommand = new NpgsqlCommand("SELECT get_unique_cast_count(@search_name)", connection))
                {
                    countCommand.Parameters.AddWithValue("search_name", searchActorName);
                    totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync());
                }

                using (var command = new NpgsqlCommand("SELECT * FROM get_cast_with_movie_count(@page_num, @page_size, @search_name)", connection))
                {
                    command.Parameters.AddWithValue("page_num", pageNum);
                    command.Parameters.AddWithValue("page_size", pageSize);
                    command.Parameters.AddWithValue("search_name", searchActorName);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new CastItem
                            {
                                PersonId = reader.GetInt32(reader.GetOrdinal("PersonId")),
                                PersonName = reader.GetString(reader.GetOrdinal("PersonName")),
                                MovieCount = reader.GetInt32(reader.GetOrdinal("MovieCount")),
                                ProfilePath = reader.GetString(reader.GetOrdinal("ProfilePath")),
                            });
                        }
                    }
                }
            }


            PaginatedResponse<CastItem> paginatedResponse = new PaginatedResponse<CastItem>(results,totalCount,pageNum,pageSize);
            
            return paginatedResponse;
        }

        public async Task<IEnumerable<PersonSearchItemDTO>> GetCastSuggestions(string searchActorName)
        {

            searchActorName = searchActorName.Trim();
            List<PersonSearchItemDTO> results = new List<PersonSearchItemDTO>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM get_cast_with_movie_count(@page_num, @page_size, @search_name)", connection))
                {
                    command.Parameters.AddWithValue("page_num", 1);
                    command.Parameters.AddWithValue("page_size", 5);
                    command.Parameters.AddWithValue("search_name", searchActorName);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(new PersonSearchItemDTO
                            {
                                PersonId = reader.GetInt32(reader.GetOrdinal("PersonId")),
                                PersonName = reader.GetString(reader.GetOrdinal("PersonName"))
                            });
                        }
                    }
                }
            }

            return results;
        }
    }
}
