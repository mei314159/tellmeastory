using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace TellMe.Mobile.Core.Types.Extensions
{
    public static class Extensions
    {
        public static IDictionary<string, string> ToKeyValue(this object metaToken)
        {
            if (metaToken == null)
            {
                return null;
            }

            JToken token = metaToken as JToken;
            if (token == null)
            {
                return ToKeyValue(JObject.FromObject(metaToken));
            }

            if (token.HasValues)
            {
                var contentData = new Dictionary<string, string>();
                foreach (var child in token.Children().ToList())
                {
                    var childContent = child.ToKeyValue();
                    if (childContent != null)
                    {
                        contentData = contentData.Concat(childContent)
                                                 .ToDictionary(k => k.Key, v => v.Value);
                    }
                }

                return contentData;
            }

            var jValue = token as JValue;
            if (jValue?.Value == null)
            {
                return null;
            }

            var value = jValue?.Type == JTokenType.Date ?
                            jValue?.ToString("o", CultureInfo.InvariantCulture) :
                            jValue?.ToString(CultureInfo.InvariantCulture);

            return new Dictionary<string, string> { { token.Path, value } };
        }

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