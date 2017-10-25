using System;
using System.IO;

namespace TellMe.Core
{
    public static class Constants
    {

#if DEBUG
        //public const string ApiHost = "http://localhost:5000";
		//public const string ApiHost = "http://192.168.0.100:5000";
        public const string ApiHost = "http://192.168.1.73:5000";
#else
		public const string ApiHost = "http://tellme-server.azurewebsites.net";
#endif

        public static string LocalDbPath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "TellMeAStory.db");

        public static string TempVideoStorage = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    }
}