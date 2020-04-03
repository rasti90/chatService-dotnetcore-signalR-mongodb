using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Model;
using ChatServer.Model.ViewModels;
using ChatServer.Service;
using ChatServer.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers {
    [Authorize]
    [Route ("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;

        public ChatController (IChatService chatService, IUserService userService) {
            this._chatService = chatService;
            this._userService = userService;
        }

        // GET: api/Chat
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserChatVM>>> Get () {
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

        // GET: api/Chat/5d41494f86f61d731f895f36
        [HttpGet ("{chatId}")]
        public async Task<ActionResult<Chat>> Get (string chatId) {
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

        // POST: api/Chat
        [HttpPost]
        public async Task<ActionResult<Chat>> Get ([FromBody] ChatVM model) {
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

    }
}