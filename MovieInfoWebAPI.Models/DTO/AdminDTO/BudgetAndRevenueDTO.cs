using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Models.DTO.AdminDTO
{
    public class BudgetAndRevenueDTO
    {
        public int ReleaseYear { get; set; }
        public double AvgBudget { get; set; }
        public double AvgRevenue { get; set; }
    }
}
