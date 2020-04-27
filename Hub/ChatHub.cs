using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatServer.Hub {
    [Authorize]
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub {
        private readonly IHubService _hubService;
        public ChatHub (IHubService hubservice) {
            this._hubService = hubservice;
        }

        public async Task SendMessage (string user, string message) {
            await Clients.All.SendAsync ("ReceiveMessage", user, message);
        }

        public async Task AddToChat (string chatId) {
            try {
                var user = Context.User as ClaimsPrincipal;
                string userId = user.GetClaimValue ("UserId");
                string appId = user.GetClaimValue ("AppId");

                var userInfo = _hubService.FindUserInChat (appId, chatId, userId);
                if (userInfo != null) {
                    await Groups.AddToGroupAsync (Context.ConnectionId, chatId);
                }
            } catch {

            }
        }

        public async Task GetChatHistory (string chatId) {
            try {
                var user = Context.User as ClaimsPrincipal;
                string userId = user.GetClaimValue ("UserId");
                string appId = user.GetClaimValue ("AppId");

                var userChatInfoWithLastConversationList = await _hubService.GetChatHistoryAndDoAppropriateActions (appId, chatId, userId);
                if (userChatInfoWithLastConversationList != null) {
                    await Groups.AddToGroupAsync (Context.ConnectionId, chatId);
                    await Clients.Caller.SendAsync ("GetChatHistory", userChatInfoWithLastConversationList.Chat);
                    await Clients.Caller.SendAsync ("GetUnreadConversationsCount", userChatInfoWithLastConversationList);
                }
            } catch {

            }
        }

        public async Task SendMessageToChat (string chatId, string message) {
            try {
                var user = Context.User as ClaimsPrincipal;
                string userId = user.GetClaimValue ("UserId");
                string appId = user.GetClaimValue ("AppId");

                var chatConversation = await _hubService.SendMessageToChat (appId, chatId, userId, message);
                if (chatConversation != null) {
                    await Clients.Group (chatId).SendAsync ("ReceiveMessageFromChat", chatId, chatConversation);
                }
            } catch {

            }
        }

        public override async Task OnConnectedAsync () {
            try {
                var user = Context.User as ClaimsPrincipal;
                string userId = user.GetClaimValue ("UserId");
                string appId = user.GetClaimValue ("AppId");

                var httpContext = Context.GetHttpContext ();
                var access_token = httpContext.Request.Query["access_token"].ToString ();
                var userInfo = await _hubService.MakeUserOnline (appId, userId, Context.ConnectionId, access_token);
                if (userInfo == null) {
                    Context.Abort ();
                }
            } catch {
                Context.Abort ();
            }
        }

        public override async Task OnDisconnectedAsync (Exception exception) {
            var user = Context.User as ClaimsPrincipal;
            string userId = user.GetClaimValue ("UserId");
            string appId = user.GetClaimValue ("AppId");

            var httpContext = Context.GetHttpContext ();
            var access_token = httpContext.Request.Query["access_token"].ToString ();

            await _hubService.MakeUserOffline (appId, userId, Context.ConnectionId, access_token);
        }

    }
}