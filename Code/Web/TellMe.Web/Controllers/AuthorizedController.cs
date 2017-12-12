using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TellMe.Web.DAL.Contracts.Services;

namespace TellMe.Web.Controllers
{
    [Authorize]
    public class AuthorizedController: Controller
    {
        protected readonly IHttpContextAccessor HttpContextAccessor;
        protected readonly IUserService UserService;
        protected string UserId => HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
		
        public AuthorizedController(IHttpContextAccessor httpContextAccessor, IUserService userService)
		{
            this.HttpContextAccessor = httpContextAccessor;
            this.UserService = userService;
        }
    }
}
