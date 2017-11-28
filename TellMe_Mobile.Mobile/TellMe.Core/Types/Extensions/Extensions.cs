using System;

namespace TellMe.Core.Types.Extensions
{
    public static class Extensions
    {
        public static DateTime GetUtcDateTime(this DateTime dateTime)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second, DateTimeKind.Utc);
        }

        public static string GetDateString(this DateTime publishDateUTC)
        {
            var utcNow = DateTime.UtcNow;
            var ago = utcNow - publishDateUTC;

            int points;
            string measure;
            if (ago.TotalMinutes < 60)
            {
                points = (int) ago.TotalMinutes;
                measure = "min";
            }
            else if (ago.TotalHours < 24)
            {
                points = (int) ago.TotalHours;
                measure = "hour";
            }
            else if (ago.TotalDays < 7)
            {
                points = (int) ago.TotalDays;
                measure = "day";
            }
            else if (ago.TotalDays < 30)
            {
                points = ((int) (ago.TotalDays / 7));
                measure = "week";
            }
            else if (ago.TotalDays < 365)
            {
                points = ((int) (ago.TotalDays / 30));
                measure = "month";
            }
            else
            {
                points = ((int) (ago.TotalDays / 365));
                measure = "year";
            }
            return string.Format("{0} {1}{2}", points, measure, points == 1 ? string.Empty : "s");
        }
    }
}