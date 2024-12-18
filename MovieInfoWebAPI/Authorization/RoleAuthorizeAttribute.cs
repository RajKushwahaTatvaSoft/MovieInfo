using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MovieInfoWebAPI.Services.Services.Helper.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Data;
using MovieInfoWebAPI.Services.Constants;
using System.Security.Claims;

namespace MovieInfoWebAPI.Authorization
{
    public class RoleAuthorize : Attribute, IAuthorizationFilter
    {
        private readonly List<int> allowedRoles;

        public RoleAuthorize(params ApiRole[] apiRoles)
        {
            allowedRoles = apiRoles.Select(r => (int)r).ToList();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (allowedRoles.Contains((int)ApiRole.AllRoles))
            {
                bool hasAnyRole = context.HttpContext.User.Claims.Any(c => c.Type == ClaimTypes.Role);
                if (hasAnyRole)
                {
                    return;
                }
                context.Result = new UnauthorizedResult();
                return;
            }


            bool isAllowed = context.HttpContext.User.Claims.Any(c => c.Type == ClaimTypes.Role && allowedRoles.Contains(Convert.ToInt32(c.Value)));

            if (!isAllowed)
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            //var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();

            //if (jwtService == null)
            //{
            //    context.HttpContext.Response.Cookies.Delete("hallodoc");
            //    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Guest", action = "Index" }));
            //    return;
            //}

            //var token = context.HttpContext.Request.Cookies["hallodoc"];

            //if (token == null || !jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            //{
            //    context.HttpContext.Response.Cookies.Delete("hallodoc");
            //    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Guest", action = "Index" }));
            //    return;
            //}

            //var roleClaim = jwtToken.Claims.FirstOrDefault(claims => claims.Type == "accountTypeId");
            //var userIdClaim = jwtToken.Claims.FirstOrDefault(claims => claims.Type == "userId");
            //var userNameClaim = jwtToken.Claims.FirstOrDefault(claims => claims.Type == "userName");
            //var userAspIdClaim = jwtToken.Claims.FirstOrDefault(claims => claims.Type == "userAspId");

            //context.HttpContext.Session.SetString("userName", userNameClaim?.Value ?? "");
            //context.HttpContext.Session.SetString("userAspId", userAspIdClaim?.Value ?? "");
            //context.HttpContext.Request.Headers.Add("userId", userIdClaim?.Value);
            //context.HttpContext.Request.Headers.Add("userName", userNameClaim?.Value);
            //context.HttpContext.Request.Headers.Add("userAspId", userAspIdClaim?.Value);

            //if (roleClaim == null)
            //{
            //    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Guest", action = "Index" }));
            //    context.HttpContext.Response.Cookies.Delete("hallodoc");
            //    return;
            //}

            //if (!(_accountTypeId == Convert.ToInt32(roleClaim.Value)))
            //{
            //    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Guest", action = "AccessDenied" }));
            //    return;
            //}

        }


    }
}
