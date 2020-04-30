using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Repository.Contract;
using Microsoft.AspNetCore.Http;

namespace ChatServer.Middleware {
    public class ClaimsCheckingMiddleware {
        private readonly RequestDelegate _next;
        public static readonly object HttpContextItemsMiddlewareUserKey = new Object();
        
        public ClaimsCheckingMiddleware (RequestDelegate next) {
            if (next == null) {
                throw new ArgumentNullException (nameof (next));
            }
            _next = next;
        }

        public async Task InvokeAsync (HttpContext context, IApplicationRepository applicationRepository, IUserRepository userRepository) {
            var user = context.User as ClaimsPrincipal;
            string userId = user.GetClaimValue ("UserId");
            string appId = user.GetClaimValue ("AppId");
            if (!string.IsNullOrEmpty (userId) && !string.IsNullOrEmpty (appId)) {
                var app = await applicationRepository.GetAsync (appId);
                if (app != null) {
                    var userInfo = await userRepository.GetAsync (appId, userId);
                    if (userInfo != null) {
                        context.Items[HttpContextItemsMiddlewareUserKey]=userInfo;
                        await _next (context);
                    }
                }
            }
        }
        
    }
}