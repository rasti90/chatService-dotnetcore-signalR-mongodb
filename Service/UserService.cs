using System.Collections.Generic;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Model;
using ChatServer.Model.Enum;
using ChatServer.Model.ViewModels;
using ChatServer.Repository.Contract;
using ChatServer.Service.Contract;
using Microsoft.AspNetCore.Http;

namespace ChatServer.Service {
    public partial class UserService : IUserService {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public UserService (IUserRepository userRepository, IChatRepository chatRepository
            , IHttpContextAccessor httpContextAccessor) {
            this._userRepository = userRepository;
            this._chatRepository = chatRepository;
            this._httpContextAccessor=httpContextAccessor;
        }

        public async Task<List<UserChatVM>> GetUserChats (string appId, string userId) {
            var chats = await _chatRepository.GetByUserIdAsync (appId, userId);
            List<UserChatVM> userChats = new List<UserChatVM> ();

            foreach (var chat in chats) {
                if (chat.ChatType == ChatType.Private) {
                    var otherUser = chat.ChatMembers.Find (chatMember => chatMember.UserId != userId);
                    var otherUserInfo = await _userRepository.GetAsync (appId, otherUser.UserId);
                    chat.Name = otherUserInfo.FullName;
                }
                userChats.Add (new UserChatVM () { UserId = userId, Chat = chat, NewMessagesCount = chat.ChatConversations.Count });
            }
            return userChats;
        }

        public async Task<User> GetUserInformation (string appId, string userId) {
            var user = await HttpContext.GetUserInfoAsync();
            return user;
        }

        public async Task<List<User>> GetUsers (string appId) {
            var users = await _userRepository.GetByAppIdAsync (appId);
            return users;
        }
    }
}