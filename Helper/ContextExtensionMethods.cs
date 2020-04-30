using System.Threading.Tasks;
using ChatServer.Middleware;
using ChatServer.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ChatServer.Helper {
    public static class ContextExtensionMethods {
        public static  User GetUserInfo (this HttpContext context) {
            context.Items
                .TryGetValue(ClaimsCheckingMiddleware.HttpContextItemsMiddlewareUserKey, 
                    out var userInfo);
            return (User)userInfo;
        }
        
        public static async Task<User> GetUserInfoAsync (this HttpContext context) {
            context.Items
                .TryGetValue(ClaimsCheckingMiddleware.HttpContextItemsMiddlewareUserKey, 
                    out var userInfo);
            return await Task.FromResult((User)userInfo);
        }
    }
}