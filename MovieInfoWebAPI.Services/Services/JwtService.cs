﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieInfoWebAPI.Models.CustomModels;
using MovieInfoWebAPI.Services.Services.Helper.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieInfoWebAPI.Services.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        public JwtService(IConfiguration configuration)
        {
            _config = configuration;
        }

        public string GenerateJwtToken(SessionUser user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name,user.UserFullName),
                new Claim(ClaimTypes.Role,user.RoleId.ToString()),
                //new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                //new Claim("roleId", user.RoleId.ToString()),
                new Claim("userFullName", user.UserFullName),
                new Claim("userId", user.UserId.ToString()),
                //new Claim("accountTypeId", user.AccountTypeId.ToString()),
                //new Claim("userAspId", user.UserAspId),
            };

            string? jwtKey = _config["Jwt:Key"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expiryTime = DateTime.UtcNow.AddDays(1);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims, expires: expiryTime,
                signingCredentials: credential
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken)
        {
            jwtSecurityToken = null;

            if (token == null)
            {
                return false;
            }

            string? jwtKey = _config["Jwt:Key"];
            if (jwtKey == null)
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtKey);
            try
            {

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                jwtSecurityToken = (JwtSecurityToken)validatedToken;

                if (jwtSecurityToken != null)
                {
                    return true;
                }

                return false;

            }
            catch
            {
                return false;
            }


        }

    }

}
