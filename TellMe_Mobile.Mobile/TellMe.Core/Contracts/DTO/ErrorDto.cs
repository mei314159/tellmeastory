using Newtonsoft.Json;

namespace TellMe.Core.Contracts.DTO
{
    public class ErrorDTO
    {
        [JsonProperty(PropertyName = "error")]
        public virtual string Error { get; set; }

        [JsonProperty(PropertyName = "message")]
        public virtual string ErrorMessage { get; set; }
    }
}