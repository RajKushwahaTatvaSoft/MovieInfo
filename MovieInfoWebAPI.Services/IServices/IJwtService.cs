using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieInfoWebAPI.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Services.Services.Helper.Interface
{
    public interface IJwtService
    {
        public string GenerateJwtToken(SessionUser user);

        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken);

    }

}