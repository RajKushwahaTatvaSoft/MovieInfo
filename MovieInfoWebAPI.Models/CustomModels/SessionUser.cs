using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Models.CustomModels
{
    public class SessionUser
    {
        public int UserId {  get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId {  get; set; }
        
    }
}
