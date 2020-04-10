using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
                var userClaim = Context.User as ClaimsPrincipal;
                string userId = userClaim.Claims.Where (c => c.Type == "UserId")
                    .Select (x => x.Value).FirstOrDefault ();
                string appId = userClaim.Claims.Where (c => c.Type == "AppId")
                    .Select (x => x.Value).FirstOrDefault ();

                var user = _hubService.FindUserInChat (appId, chatId, userId);
                if (user != null) {
                    await Groups.AddToGroupAsync (Context.ConnectionId, chatId);
                }
            } catch {

            }
        }

        public async Task GetChatHistory (string chatId) {
            try {
                var userClaim = Context.User as ClaimsPrincipal;
                string userId = userClaim.Claims.Where (c => c.Type == "UserId")
                    .Select (x => x.Value).FirstOrDefault ();
                string appId = userClaim.Claims.Where (c => c.Type == "AppId")
                    .Select (x => x.Value).FirstOrDefault ();

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
                var userClaim = Context.User as ClaimsPrincipal;
                string userId = userClaim.Claims.Where (c => c.Type == "UserId")
                    .Select (x => x.Value).FirstOrDefault ();
                string appId = userClaim.Claims.Where (c => c.Type == "AppId")
                    .Select (x => x.Value).FirstOrDefault ();

                var chatConversation = await _hubService.SendMessageToChat (appId, chatId, userId, message);
                if (chatConversation != null) {
                    await Clients.Group (chatId).SendAsync ("ReceiveMessageFromChat", chatId, chatConversation);
                }
            } catch {

            }
        }

        public override async Task OnConnectedAsync () {
            try {
                var userClaim = Context.User as ClaimsPrincipal;
                string userId = userClaim.Claims.Where (c => c.Type == "UserId")
                    .Select (x => x.Value).FirstOrDefault ();
                string appId = userClaim.Claims.Where (c => c.Type == "AppId")
                    .Select (x => x.Value).FirstOrDefault ();

                var httpContext = Context.GetHttpContext ();
                var access_token = httpContext.Request.Query["access_token"].ToString ();
                var user = await _hubService.MakeUserOnline (appId, userId, Context.ConnectionId, access_token);
                if (user == null) {
                    Context.Abort ();
                }
            } catch {
                Context.Abort ();
            }
        }

        public override async Task OnDisconnectedAsync (Exception exception) {
            var userClaim = Context.User as ClaimsPrincipal;
            string userId = userClaim.Claims.Where (c => c.Type == "UserId")
                .Select (x => x.Value).FirstOrDefault ();
            string appId = userClaim.Claims.Where (c => c.Type == "AppId")
                .Select (x => x.Value).FirstOrDefault ();

            var httpContext = Context.GetHttpContext ();
            var access_token = httpContext.Request.Query["access_token"].ToString ();

            await _hubService.MakeUserOffline (appId, userId, Context.ConnectionId, access_token);
        }

    }
}