using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using TellMe.Web.DAL.DTO;
using TellMe.Web.DTO;

namespace TellMe.Web
{
    public class AccountDTOFormatter : InputFormatter
    {
        public AccountDTOFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
        }
        protected override bool CanReadType(Type type)
        {
            return type == typeof(AccountDTO);
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var result = new AccountDTO();

            var file = context.HttpContext.Request.Form.Files.FirstOrDefault();
            if (file != null)
            {
                var fileStream = file.OpenReadStream();
                var resultFile = new FormFile(fileStream, 0, file.Length, file.Name, file.FileName);
                resultFile.Headers = file.Headers;
                result.File = resultFile;
            }

            result.User = JsonConvert.DeserializeObject<UserDTO>(context.HttpContext.Request.Form["User"]);
            return InputFormatterResult.SuccessAsync(result);
        }
    }
}