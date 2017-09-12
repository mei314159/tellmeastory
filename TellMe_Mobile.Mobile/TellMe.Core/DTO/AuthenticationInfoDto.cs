using System;
using Newtonsoft.Json;

namespace TellMe.Core.DTO
{
    public class AuthenticationInfoDTO
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

		[JsonProperty(PropertyName = "refresh_token")]
		public string RefreshToken { get; set; }

		[JsonProperty(PropertyName = "userid")]
        public string UserId { get; set; }

        public DateTime AuthDate { get; set; }
    }

    
}