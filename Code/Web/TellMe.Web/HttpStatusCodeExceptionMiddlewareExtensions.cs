using Microsoft.AspNetCore.Builder;

namespace TellMe.Web
{
    public static class TransactionAwareExceptionMiddleware
    {
        public static IApplicationBuilder UseTransactionAwareExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpStatusCodeExceptionMiddleware>();
        }
    }
}