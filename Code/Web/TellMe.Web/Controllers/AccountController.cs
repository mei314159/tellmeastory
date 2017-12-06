using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TellMe.Web.DTO;
using Microsoft.AspNetCore.Hosting;
using TellMe.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net;
using TellMe.Web.DAL.Contracts.Services;
using TellMe.Web.DAL.Types.Domain;

namespace TellMe.Web.Controllers
{
    [Route("api/account")]
    public class AccountController : AuthorizedController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _environment;
        private readonly IStorageService _storageService;
        private readonly IUserService _userService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment environment,
            IStorageService storageService,
            IHttpContextAccessor httpContextAccessor, IUserService userService, IUserService userService1) :
            base(httpContextAccessor, userService)
        {
            this._userManager = userManager;
            _environment = environment;
            _storageService = storageService;
            _userService = userService1;
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
                var applicationUser = new ApplicationUser
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    EmailConfirmed = false,
                    // PhoneNumber = dto.PhoneNumber,
                    // PhoneNumberDigits = long.Parse(formattedPhoneNumber),
                    // PhoneNumberConfirmed = true, //Must be false and confirmed separately by sms
                    // PhoneCountryCode = dto.PhoneCountryCode,
                    // CountryCode = dto.CountryCode
                };
                var result = await _userManager.CreateAsync(applicationUser, dto.Password);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                    await _userService.SendRegistrationConfirmationEmailAsync(applicationUser.Id, applicationUser.Email,
                        token);
                    return Ok();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpGet("confirm/{userId}/{code}")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest("User was not found");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return Ok();
            }

            foreach (var identityError in result.Errors)
            {
                ModelState.AddModelError(identityError.Code, identityError.Description);
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

            return this.StatusCode((int) HttpStatusCode.InternalServerError);
        }
    }
}