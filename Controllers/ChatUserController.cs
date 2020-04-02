using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChatServer.Service;
using ChatServer.Model;
using ChatServer.Model.ViewModels;
using ChatServer.Helper;
using Microsoft.AspNetCore.Authorization;
using ChatServer.Service.Contract;
using System.Security.Claims;

namespace ChatServer.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatUserController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatUserController(IChatService chatService)
        {
            this._chatService=chatService;
        }

        // GET: api/ChatUser/5d41494f86f61d731f895f36
        [HttpGet("{chatId}")]
        public async Task<ActionResult<IEnumerable<ChatMember>>> Get(string chatId)
        {
            try{
                var user = User as ClaimsPrincipal;
                string appId = user.GetClaimValue("AppId");

                var chatMembers=await _chatService.GetChatMembers(appId, chatId);
                return chatMembers;
            }catch{

            }
            return NotFound();
        }

        // POST: api/ChatUser
        [HttpPost]
        public async Task<ActionResult<List<ChatMember>>> Get([FromBody] ChatMembersVM model)
        {
            try{
                var user = User as ClaimsPrincipal;
                string appId = user.GetClaimValue("AppId");

                var addedMembers=await _chatService.AddMembersToChat(appId, model);
                return addedMembers;
            }
            catch{

            }
            return NotFound();
        }

    }
}