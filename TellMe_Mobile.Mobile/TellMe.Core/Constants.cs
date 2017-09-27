using System;
using System.IO;

namespace TellMe.Core
{
    public static class Constants
    {
        //public const string ApiHost = "http://localhost:5000";
        public const string ApiHost = "http://tellme-server.azurewebsites.net";

        public static string LocalDbPath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "TellMeAStory.db");
    }
}
