using Newtonsoft.Json;

namespace TellMe.Core.Dto
{

    public class ErrorDTO
    {
        [JsonProperty(PropertyName = "error")]
        public virtual string Error { get; set; }

        [JsonProperty(PropertyName = "message")]
        public virtual string ErrorMessage { get; set; }
    }


    public class AuthenticationErrorDto : ErrorDTO
    {
        [JsonProperty(PropertyName = "error_description")]
        public override string ErrorMessage { get; set; }
    }
}