using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Model;
using ChatServer.Model.ViewModels;
using ChatServer.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers {
    [Authorize]
    [Route ("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;

        public ChatsController (IChatService chatService, IUserService userService) {
            this._chatService = chatService;
            this._userService = userService;
        }

        // GET: api/chats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserChatVM>>> GetUserChats () {
            try {
                var user = User as ClaimsPrincipal;
                string userId = user.GetClaimValue ("UserId");
                string appId = user.GetClaimValue ("AppId");

                var userChats = await _userService.GetUserChats (appId, userId);
                return userChats;
            } catch {

            }
            return NotFound ();
        }

        // GET: api/chats/5d41494f86f61d731f895f36
        [HttpGet ("{chatId}")]
        public async Task<ActionResult<Chat>> GetChat (string chatId) {
            try {
                var user = User as ClaimsPrincipal;
                string userId = user.GetClaimValue ("UserId");
                string appId = user.GetClaimValue ("AppId");

                var chat = await _chatService.GetChatInformation (appId, userId, chatId);
                if (chat != null) {
                    return chat;
                }
            } catch {

            }
            return NotFound ();
        }

        // POST: api/chats
        [HttpPost]
        public async Task<ActionResult<Chat>> CreateChat ([FromBody] ChatVM model) {
            try {
                var user = User as ClaimsPrincipal;
                string userId = user.GetClaimValue ("UserId");
                string appId = user.GetClaimValue ("AppId");

                var chat = await _chatService.CreateChat (appId, userId, model);
                if (chat != null) {
                    return chat;
                }
            } catch {

            }
            return NotFound ();
        }

        // GET: api/chats/5d41494f86f61d731f895f36/members
        [HttpGet ("{chatId}/members")]
        public async Task<ActionResult<IEnumerable<ChatMember>>> GetChatMembers (string chatId) {
            try {
                var user = User as ClaimsPrincipal;
                string appId = user.GetClaimValue ("AppId");

                var chatMembers = await _chatService.GetChatMembers (appId, chatId);
                return chatMembers;
            } catch {

            }
            return NotFound ();
        }

        // POST: api/chats/5d41494f86f61d731f895f36/members
        [HttpPost ("{chatId}/members")]
        public async Task<ActionResult<List<ChatMember>>> AddMemberToChat (string chatId, [FromBody] List<ChatMember> members) {
            try {
                var user = User as ClaimsPrincipal;
                string appId = user.GetClaimValue ("AppId");

                var model = new ChatMembersVM () { ChatId = chatId, ChatMembers = members };
                var addedMembers = await _chatService.AddMembersToChat (appId, model);
                return addedMembers;
            } catch {

            }
            return NotFound ();
        }

    }
}