using ChatServer.Middleware;
using Microsoft.AspNetCore.Builder;

namespace ChatServer.Helper {
    public static class MiddlewareExtensionMethods {
        public static IApplicationBuilder UseClaimChecking (this IApplicationBuilder builder) {
            return builder.UseWhen (context => context.Request.Headers.ContainsKey ("Authorization"),
                builder => builder.UseMiddleware<ClaimsCheckingMiddleware> ());
        }
    }
}