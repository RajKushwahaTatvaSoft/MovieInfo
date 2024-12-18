using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Models.DTO.CrewDTO
{
    public class CrewItemDTO
    {
        public List<int> PersonIds { get; set; } = new List<int>();
        public int JobId { get; set; }
        public int DepartmentId { get; set; }
    }
}
