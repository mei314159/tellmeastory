using Microsoft.AspNetCore.Http;

namespace TellMe.Web.DTO
{
    public class FileInputDTO
    {
        public IFormFile VideoFile { get; set; }

        public IFormFile PreviewImageFile { get; set; }
    }
}
