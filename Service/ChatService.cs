using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;
using ChatServer.Model.Enum;
using ChatServer.Model.ViewModels;
using ChatServer.Repository.Contract;
using ChatServer.Service.Contract;

namespace ChatServer.Service {
    public partial class ChatService : IChatService {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;

        private readonly IUserService _userService;

        public ChatService (IUserRepository userRepository, IChatRepository chatRepository
            , IUserService userService) {
            this._userRepository = userRepository;
            this._chatRepository = chatRepository;
            this._userService = userService;
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
    }
}