using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Types.Domain;
using TellMe.Web.DTO;

namespace TellMe.Web.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        public AccountController(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignupAsync([FromBody] SignUpDTO dto)
        {
            if (dto != null && ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                }, dto.Password);
                if (result.Succeeded)
                {
                    return Ok();
                } 

                foreach (var error in result.Errors){
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            return BadRequest();
        }
    }
}