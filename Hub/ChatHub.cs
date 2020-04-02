using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System.Threading.Tasks;
using ChatServer.Model;
using ChatServer.Model.ViewModels;
using ChatServer.Service;
using System.Collections.Generic;
using System;
using System.Linq;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using ChatServer.Service.Contract;
using System.Security.Claims;

namespace ChatServer.Hub
{
    [Authorize]
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IHubService _hubService;
        public ChatHub(IHubService hubservice)
        {
            this._hubService=hubservice;
        }
        
        public async Task SendMessage(string user, string message)
        {                      
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task AddToChat(string chatId)
        {
            try{
                var userClaim = Context.User as ClaimsPrincipal;
                string userId = userClaim.Claims.Where(c => c.Type == "UserId")
                    .Select(x => x.Value).FirstOrDefault();
                string appId = userClaim.Claims.Where(c => c.Type == "AppId")
                    .Select(x => x.Value).FirstOrDefault();

                var user=_hubService.FindUserInChat(appId,chatId,userId);
                if(user!=null){
                    await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
                }
            }
            catch{

            }
        }

        public async Task GetChatHistory(string chatId)
        {
            try
            {
                var userClaim = Context.User as ClaimsPrincipal;
                string userId = userClaim.Claims.Where(c => c.Type == "UserId")
                    .Select(x => x.Value).FirstOrDefault();
                string appId = userClaim.Claims.Where(c => c.Type == "AppId")
                    .Select(x => x.Value).FirstOrDefault();

                var userChatInfoWithLastConversationList=await _hubService.GetChatHistoryAndDoAppropriateActions(appId,chatId,userId);
                if(userChatInfoWithLastConversationList!=null){
                    await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
                    await Clients.Caller.SendAsync("GetChatHistory", userChatInfoWithLastConversationList.Chat);
                    await Clients.Caller.SendAsync("GetUnreadConversationsCount", userChatInfoWithLastConversationList);
                }
            }
            catch
            {

            }
        }

        public async Task SendMessageToChat(string chatId, string message)
        {
            try{
                var userClaim = Context.User as ClaimsPrincipal;
                string userId = userClaim.Claims.Where(c => c.Type == "UserId")
                    .Select(x => x.Value).FirstOrDefault();
                string appId = userClaim.Claims.Where(c => c.Type == "AppId")
                    .Select(x => x.Value).FirstOrDefault();

                var chatConversation=await _hubService.SendMessageToChat(appId,chatId,userId,message);
                if(chatConversation!=null){
                    await Clients.Group(chatId).SendAsync("ReceiveMessageFromChat", chatId, chatConversation);
                }
            }catch{

            }
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userClaim = Context.User as ClaimsPrincipal;
                string userId = userClaim.Claims.Where(c => c.Type == "UserId")
                    .Select(x => x.Value).FirstOrDefault();
                string appId = userClaim.Claims.Where(c => c.Type == "AppId")
                    .Select(x => x.Value).FirstOrDefault();

                //var userExternalId = Context.User.Claims.FirstOrDefault(c => c.ToString()=="userId").Value;
                var httpContext = Context.GetHttpContext();
                //var APIKey = httpContext.Request.Query["APIKey"].ToString();
                var access_token= httpContext.Request.Query["access_token"].ToString();
                var user = await _hubService.MakeUserOnline(appId, userId, Context.ConnectionId, access_token);
                if(user==null){
                    Context.Abort();
                }
            }
            catch
            {
                Context.Abort();
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // var httpContext = Context.GetHttpContext();
            // var userExternalId = httpContext.Request.Query["userExternalId"].ToString();
            // var APIKey = httpContext.Request.Query["APIKey"].ToString();
            // var app = await _applicationService.GetByAPIKeyAsync(APIKey);
            // var user = await _userService.GetByExternalIdAsync(userExternalId);
            // if (user != null && app != null)
            // {
            //     var connection = user.connections.FirstOrDefault(conn => conn.connectionId == Context.ConnectionId);
            //     var activity = new Activity() { appId = app.Id, activityType = Models.Enums.ActivityType.getOffline, connectionId = Context.ConnectionId, date = DateTime.Now };
            //     await _userService.AddActivityAndManageConnectionToUserAsync(user.Id, activity, connection);
            // }
            
        }
    }
}