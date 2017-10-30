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
using TellMe.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace TellMe.Web.Controllers
{
    [Route("api/account")]
    public class AccountController : AuthorizedController
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _environment;
        private readonly IStorageService _storageService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment environment,
            IStorageService storageService,
            IHttpContextAccessor httpContextAccessor, IUserService userService) :
             base(httpContextAccessor, userService)
        {
            this._userManager = userManager;
            _environment = environment;
            _storageService = storageService;
        }

        [HttpGet("env")]
        public IActionResult GetEnvironment()
        {
            return Ok(_environment.EnvironmentName);
        }

        [AllowAnonymous, HttpPost("signup")]
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

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost("picture")]
        public async Task<IActionResult> SetPictureAsync(ProfilePictureInputDTO dto)
        {
            if (dto == null)
                return BadRequest("Argument null");

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("Picture is null");

            var user = await _userManager.FindByIdAsync(this.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var blobName = dto.File.GetFilename();
            var fileStream = await dto.File.GetFileStream();

            var uploadResult = await _storageService.UploadProfilePictureAsync(fileStream, blobName);
            user.PictureUrl = uploadResult.PictureUrl;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(uploadResult);
            }
            else
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}