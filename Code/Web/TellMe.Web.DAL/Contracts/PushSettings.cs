namespace TellMe.Web.DAL.Contracts
{
    public class PushSettings
    {
        public string Certificate { get; set; }

        public string Password { get; set; }

        public bool IsProductionMode { get; set; }
    }
}