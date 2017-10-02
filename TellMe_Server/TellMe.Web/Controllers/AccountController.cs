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

        [HttpPost("signup-phone")]
        public async Task<IActionResult> SignupPhoneAsync([FromBody] SignUpPhoneDTO dto)
        {
            if (dto != null && ModelState.IsValid)
            {
                var formattedPhoneNumber = new Regex(Constants.PhoneNumberCleanupRegex).Replace(dto.PhoneNumber, string.Empty);
                var existingUser = await _userManager.FindByNameAsync(formattedPhoneNumber);
                if (existingUser != null){
                    // TODO Set new confirmation code and send to user by sms
                    return Ok();
                }

                var result = await _userManager.CreateAsync(new ApplicationUser
                {
                    PhoneNumber = dto.PhoneNumber,
                    PhoneNumberDigits = long.Parse(formattedPhoneNumber),
                    PhoneNumberConfirmed = true, //Must be false and confirmed separately by sms
                    UserName = formattedPhoneNumber,
                    Email = null,
                    PhoneCountryCode = dto.PhoneCountryCode,
                    CountryCode = dto.CountryCode
                }, "0000" // temporary used instead of sms-code
                );
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