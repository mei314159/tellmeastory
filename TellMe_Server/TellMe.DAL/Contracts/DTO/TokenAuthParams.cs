namespace TellMe.DAL.Contracts.DTO
{
    public class TokenAuthParams
    {
        public string grant_type { get; set; }
        public string refresh_token { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string username { get; set; }
        public string phone_number { get; set; }
        public string confirmation_code { get; set; }
        public string password { get; set; }
    }
}
