using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Models.CustomTableModels
{
    public class CastItem
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public int MovieCount {  get; set; }
        public string? ProfilePath { get; set; }
    }
}
