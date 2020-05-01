using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;
using ChatServer.Model.Enum;
using ChatServer.Model.ViewModels;
using ChatServer.Repository.Contract;
using ChatServer.Service.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace ChatServer.Service {
    public partial class ChatService : IChatService {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;

        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;
        protected HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public ChatService (IUserRepository userRepository, IChatRepository chatRepository
            , IUserService userService, IHttpContextAccessor httpContextAccessor
            , LinkGenerator linkGenerator) {
            this._userRepository = userRepository;
            this._chatRepository = chatRepository;
            this._userService = userService;
            this._httpContextAccessor=httpContextAccessor;
            this._linkGenerator=linkGenerator;
        }

        public async Task<List<ChatMember>> AddMembersToChat (string appId, ChatMembersVM model) {         
            var chat = await _chatRepository.GetAsync (appId, model.ChatId);
            if (chat != null && chat.ChatType == ChatType.Group) {
                chat.ChatMembers = chat.ChatMembers != null ? chat.ChatMembers : new List<ChatMember> ();
                var shouldBeAdded = model.ChatMembers
                    .Where (member => !chat.ChatMembers.Any (oldMember => oldMember.UserId == member.UserId))
                    .Select (member => new ChatMember () {
                        UserId = member.UserId
                    }).ToList ();
                chat.ChatMembers.AddRange (shouldBeAdded);

                await _chatRepository.UpdateAsync (chat.Id, chat);
                return shouldBeAdded;
            }
            return null;
        }

        public async Task<Chat> CreateChat (string appId, string userId, ChatVM model) {           
            model.AppId = appId;
            model.UserId = userId;
            var chat = await CreateChatRecord (model);
            return chat;
        }

        public async Task<Chat> GetChatInformation (string appId, string userId, string chatId) {
            var chat = await _chatRepository.GetAsync (appId, chatId);
            if (chat != null) {
                if (chat.ChatMembers.Any (member => member.UserId == userId)) {
                    if (chat.ChatType == ChatType.Private) {
                        var otherUser = chat.ChatMembers.Find (chatMember => chatMember.UserId != userId);
                        var otherUserInfo = await _userRepository.GetAsync (appId, otherUser.UserId);
                        chat.Name = otherUserInfo.FullName;
                    }
                    return chat;
                }
            }
            return null;
        }

        public async Task<List<ChatMember>> GetChatMembers (string appId, string chatId) {
            var chatMembers = await _chatRepository.GetChatMembersAsync (appId, chatId);
            return chatMembers;
        }

        public async Task<ConversationFilteredResultVM> GetChatConversations (ChatHistoryFilterModel filter) {
            var chatConversations = await _chatRepository.GetChatConversationsAsync (filter);
            ConversationFilteredResultVM result=new ConversationFilteredResultVM(){Result=chatConversations}; 
            
            filter.EdgeDateTime=chatConversations.FirstOrDefault()?.Date??DateTime.Now;
            filter.DirectionType=KeysetFilterModelType.Previous;
            var previousUrl = _linkGenerator.GetUriByAction(HttpContext,null,null,filter,HttpContext.Request.Scheme);
            result.PreviousUri=new Uri(previousUrl);

            filter.EdgeDateTime=chatConversations.LastOrDefault()?.Date??DateTime.Now;
            filter.DirectionType=KeysetFilterModelType.Next;
            var nextUrl = _linkGenerator.GetUriByAction(HttpContext,null,null,filter,HttpContext.Request.Scheme);
            result.NextUri=new Uri(nextUrl);
            
            return result;
        }
    }
}