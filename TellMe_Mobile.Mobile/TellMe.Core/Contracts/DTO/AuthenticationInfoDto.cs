using System;
using Newtonsoft.Json;

namespace TellMe.Core.Contracts.DTO
{
    [SQLite.Table("AuthenticationInfo")]
    public class AuthenticationInfoDTO
    {
        [SQLite.PrimaryKey]
        [JsonProperty(PropertyName = "user_id")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        public DateTime AuthDate { get; set; }
    }
}