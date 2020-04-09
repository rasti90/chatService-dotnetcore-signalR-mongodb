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
    public class HubService : IHubService {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserProfileService _userProfileService;

        public HubService (IUserRepository userRepository, IChatRepository chatRepository, IApplicationRepository applicationRepository, IUserProfileService userProfileService) {
            this._userRepository = userRepository;
            this._chatRepository = chatRepository;
            this._applicationRepository = applicationRepository;
            this._userProfileService = userProfileService;
        }

        public async Task<User> MakeUserOnline (string appId, string userId, string connectionId, string access_token) {
            var app = await _applicationRepository.GetAsync (appId);

            if (app != null) {
                var user = await _userRepository.GetAsync (appId, userId);
                var connection = new Connection () { ConnectionId = connectionId, JWTToken = access_token };
                var activity = new Activity () { ActivityType = Model.Enum.ActivityType.getOnline, ConnectionId = connectionId, Date = DateTime.Now };
                await _userRepository.AddActivityAndManageConnectionToUserAsync (user.Id, activity, connection);

                return user;
            }
            return null;
        }

        public async Task<User> FindUserInChat (string appId, string chatId, string userId) {
            var app = await _applicationRepository.GetAsync (appId);
            if (app != null) {
                var chat = await _chatRepository.GetAsync (app.Id, chatId);
                var user = await _userRepository.GetAsync (appId, userId);
                if (chat != null && user != null) {
                    if (chat.ChatMembers.Any (member => member.UserId == user.Id)) {
                        return user;
                    }
                }
            }
            return null;
        }

        public async Task<UserChatVM> GetChatHistoryAndDoAppropriateActions (string appId, string chatId, string userId) {
            var app = await _applicationRepository.GetAsync (appId);
            if (app != null) {
                var chat = await _chatRepository.GetAsync (app.Id, chatId);
                var user = await _userRepository.GetAsync (appId, userId);
                if (chat != null && user != null) {
                    if (chat.ChatMembers.Any (member => member.UserId == user.Id)) {
                        var lastChatConversations = await _chatRepository.GetChatConversationsAsync (app.Id, chatId, 0, 100);

                        var unReadChatconversations = await _chatRepository.GetUnReadChatConversationsAsync (app.Id, chatId, user.Id);
                        foreach (var conversation in unReadChatconversations) {
                            await _chatRepository.AddReaderToConversationAsync (chatId, conversation.Id,
                                new ChatConversationReader () { UserId = user.Id, Date = DateTime.Now });
                        }
                        if (chat.ChatType == ChatType.Private) {
                            var otherUser = chat.ChatMembers.Find (chatMember => chatMember.UserId != userId);
                            var otherUserInfo = await _userRepository.GetAsync (appId, otherUser.UserId);
                            chat.Name = otherUserInfo.FullName;
                        }
                        return new UserChatVM () {
                            UserId = user.Id, Chat = new Chat { Id = chatId, Name = chat.Name, ChatConversations = lastChatConversations, ChatType = chat.ChatType }, NewMessagesCount = 0
                        };
                    }
                }
            }
            return null;
        }

        public async Task<ChatConversation> SendMessageToChat (string appId, string chatId, string userId, string message) {
            var app = await _applicationRepository.GetAsync (appId);
            if (app != null) {
                var user = await _userRepository.GetAsync (appId, userId);
                var chat = await _chatRepository.GetAsync (appId, chatId);
                if (chat != null) {
                    var chatConversation = new ChatConversation () {
                        Id = ObjectId.GenerateNewId ().ToString (),
                        UserId = user.Id,
                        Date = DateTime.Now,
                        Text = message,
                        ChatConversationReaders = new List<ChatConversationReader> (),
                            User = new User () {
                            Id = user.Id,
                            FullName = user.FullName
                        }
                    };

                    await _chatRepository.AddMessageToChatAsync (chatId, chatConversation);
                    return chatConversation;
                }
            }
            return null;
        }
    }
}