using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TellMe.DAL.Contracts.Services;
using TellMe.Web.DTO;

namespace TellMe.Web.Controllers
{

    [Route("api/contacts")]
    public class ContactsController : AuthorizedController
    {
        private readonly IContactService _contactsService;
        public ContactsController(
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService,
            IContactService contactsService) : base(httpContextAccessor, userService)
        {
            _contactsService = contactsService;
        }

        [HttpPost("synchronize")]
        public async Task<IActionResult> SynchronizeAsync([FromBody] SynchronizeContactsDTO dto)
        {
            await _contactsService.SaveContactsAsync(this.UserId, dto.Contacts);

            return Ok();
        }

        [HttpPost("app-users")]
        public async Task<IActionResult> GetContactsUsingAppAsync([FromBody] SynchronizeContactsDTO dto)
        {
            await _contactsService.GetAllAsync(this.UserId);
            return Ok();
        }
    }
}
