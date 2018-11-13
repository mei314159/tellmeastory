using System;
using System.IO;

namespace TellMe.Mobile.Core
{
    public static class Constants
    {
#if DEBUG
        public const string ApiHost = "http://tellme-server.azurewebsites.net";
        //public const string ApiHost = "http://10.211.55.3:5000";
        //public const string ApiHost = "http://localhost:5000";
        //public const string ApiHost = "http://192.168.0.100:5000";
        //public const string ApiHost = "http://192.168.1.10:5000";
        //public const string ApiHost = "http://tellme-server.azurewebsites.net";
#else
		public const string ApiHost = "http://tellme-server.azurewebsites.net";
#endif

        public static readonly string LocalDbPath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "TellMeAStory.db");

        public const string TermsAndConditionsLink = "https://termsfeed.com/privacy-policy/32d828e78e777b9dcb3bfd66df7fabbe";
        public static readonly string TempVideoStorage = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    }
}