using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Models.DTO.CastDTO
{
    public class CastItemDTO
    {
        public int PersonId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
    }
}
