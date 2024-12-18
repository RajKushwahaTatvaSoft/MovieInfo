using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Models.DTO.PersonDTO
{
    public class PersonSearchItemDTO
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
    }
}
