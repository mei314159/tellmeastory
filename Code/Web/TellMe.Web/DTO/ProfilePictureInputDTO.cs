using Microsoft.AspNetCore.Http;
using TellMe.Web.DAL.DTO;

namespace TellMe.Web.DTO
{
    public class ProfilePictureInputDTO
    {
        public IFormFile File { get; set; }
    }
    
    public class AccountDTO
    {
        public IFormFile File { get; set; }
        
        public UserDTO User { get; set; }
    }
}
