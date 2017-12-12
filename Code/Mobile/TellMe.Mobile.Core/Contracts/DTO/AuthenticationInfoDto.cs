using System;
using Newtonsoft.Json;
using SQLiteNetExtensions.Attributes;

namespace TellMe.Mobile.Core.Contracts.DTO
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
        
        [JsonProperty(PropertyName = "expires_at")]
        public DateTime ExpiresAt { get; set; }
        
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty(PropertyName = "account")]
        [TextBlob("AccountBlobbed")]
        public UserDTO Account { get; set; }

        public string AccountBlobbed { get; set; }

        [JsonIgnore]
        [SQLite.Ignore]
        public bool Expired => DateTime.UtcNow > ExpiresAt;
    }
}