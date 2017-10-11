using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TellMe.DAL.Contracts.Services;
using TellMe.DAL.Types.Domain;
using TellMe.Web.DTO;
using TellMe.DAL;
using Microsoft.AspNetCore.Hosting;

namespace TellMe.Web.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _environment;
        public AccountController(UserManager<ApplicationUser> userManager, IHostingEnvironment environment)
        {
            this._userManager = userManager;
            _environment = environment;
        }

        [HttpGet("env")]
        public IActionResult GetEnvironment()
        {
            return Ok(_environment.EnvironmentName);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignupAsync([FromBody] SignUpDTO dto)
        {
            if (dto != null && ModelState.IsValid)
            {
                //var formattedPhoneNumber = new Regex(Constants.PhoneNumberCleanupRegex).Replace(dto.PhoneNumber, string.Empty);
                var result = await _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    // PhoneNumber = dto.PhoneNumber,
                    // PhoneNumberDigits = long.Parse(formattedPhoneNumber),
                    // PhoneNumberConfirmed = true, //Must be false and confirmed separately by sms
                    // PhoneCountryCode = dto.PhoneCountryCode,
                    // CountryCode = dto.CountryCode
                }, dto.Password);

                if (result.Succeeded)
                {
                    return Ok();
                }

                foreach (var error in result.Errors){
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            return BadRequest(ModelState);
        }
    }
}