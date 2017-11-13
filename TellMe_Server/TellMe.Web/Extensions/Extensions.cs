using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace TellMe.Web.Extensions
{
    public static class IFormFileExtensions
    {
        public static string GetFilename(this IFormFile file) =>
        ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.ToString().Trim('"');

        public static async Task<MemoryStream> GetFileStream(this IFormFile file)
        {
            MemoryStream filestream = new MemoryStream();
            await file.CopyToAsync(filestream);
            return filestream;
        }

        public static async Task<byte[]> GetFileArray(this IFormFile file)
        {
            MemoryStream filestream = new MemoryStream();
            await file.CopyToAsync(filestream);
            return filestream.ToArray();
        }

        public static DateTime GetUtcDateTime(this long ticksUtc){
            var dateFromTicks = new DateTime(ticksUtc);
            return new DateTime(
                dateFromTicks.Year,
                dateFromTicks.Month,
                dateFromTicks.Day,
                dateFromTicks.Hour,
                dateFromTicks.Minute,
                dateFromTicks.Second, DateTimeKind.Utc);
        }
    }
}