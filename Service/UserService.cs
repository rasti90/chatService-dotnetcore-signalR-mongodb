using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;
using ChatServer.Model.Enum;
using ChatServer.Model.ViewModels;
using ChatServer.Repository.Contract;
using ChatServer.Service.Contract;
using MongoDB.Bson;

namespace ChatServer.Service {
    public partial class UserService : IUserService {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IApplicationRepository _applicationRepository;

        public UserService (IUserRepository userRepository, IChatRepository chatRepository, IApplicationRepository applicationRepository) {
            this._userRepository = userRepository;
            this._chatRepository = chatRepository;
            this._applicationRepository = applicationRepository;
        }

        public async Task<List<UserChatVM>> GetUserChats (string appId, string userId) {
            var app = await _applicationRepository.GetAsync(appId);
            if (app != null) {
                var user = await _userRepository.GetAsync(appId, userId);
                if (user != null) {
                    var chats = await _chatRepository.GetByUserIdAsync(app.Id, user.Id);
                    List<UserChatVM> userChats = new List<UserChatVM> ();

                    foreach (var chat in chats) {
                        if (chat.ChatType == ChatType.Private) {
                            var otherUser = chat.ChatMembers.Find (chatMember => chatMember.UserId != userId);
                            var otherUserInfo = await _userRepository.GetAsync (appId, otherUser.UserId);
                            chat.Name = otherUserInfo.FullName;
                        }
                        userChats.Add (new UserChatVM () { UserId = user.Id, Chat = chat, NewMessagesCount = chat.ChatConversations.Count });
                    }
                    return userChats;
                }
            }
            return null;
        }

        public async Task<User> GetUserInformation(string appId, string userId)
        {
            var app = await _applicationRepository.GetAsync(appId);
            if (app != null) {
                var user = await _userRepository.GetAsync(appId, userId);
                return user;
            }
            return null;
        }

        public async Task<List<User>> GetUsers(string appId)
        {
            var app = await _applicationRepository.GetAsync (appId);
            if (app != null) {
                var users = await _userRepository.GetByAppIdAsync(app.Id);
                return users;
            }
            return new List<User>();
        }
    }
}