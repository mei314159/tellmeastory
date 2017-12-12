using Microsoft.AspNetCore.Http;

namespace TellMe.Web.DTO
{
    public class ProfilePictureInputDTO
    {
        public IFormFile File { get; set; }
    }
}
