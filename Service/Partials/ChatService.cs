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
    public partial class ChatService : IChatService {
        private async Task<Chat> CreateChatRecord (ChatVM model) {
            Chat chat;
            if (model.ChatType == ChatType.Private) {
                chat = await CreatePrivateChat (model);
            } else {
                chat = await CreateGroupChat (model);
            }
            return chat;
        }

        private async Task<Chat> CreatePrivateChat (ChatVM model) {

            if (IsNotValidForCreatingPrivateChat (model)) {
                return null;
            }
            Chat chat;
            if ((chat = await IsDuplicatePVChat (model)) != null) {
                return chat;
            }
            chat = new Chat () {
                AppId = model.AppId,
                ChatType = model.ChatType,
                ChatMembers = new List<ChatMember> () {
                new ChatMember () { UserId = model.UserId },
                new ChatMember () { UserId = model.PrivateChatDetail.OtherMember }
                },
                ChatConversations = new List<ChatConversation> ()
            };
            return await _chatRepository.CreateAsync (chat);
        }

        private bool IsNotValidForCreatingPrivateChat (ChatVM model) {
            if (model.PrivateChatDetail == null) {
                return true;
            }
            if (model.UserId == model.PrivateChatDetail.OtherMember) {
                return true;
            }
            return false;
        }
        private async Task<Chat> IsDuplicatePVChat (ChatVM model) {
            var userChats = await _userService.GetUserChats (model.AppId, model.UserId);
            var duplicateUserChat = userChats.Find (chat => chat.Chat.ChatType == ChatType.Private &&
                chat.Chat.ChatMembers.Any (m => m.UserId == model.UserId) &&
                chat.Chat.ChatMembers.Any (m => m.UserId == model.PrivateChatDetail.OtherMember));
            return duplicateUserChat != null?duplicateUserChat.Chat : null;
        }

        private async Task<Chat> CreateGroupChat (ChatVM model) {
            if (IsNotValidForCreatingGroupChat (model)) {
                return null;
            }
            var chatMembers = model.GroupChatDetail.OtherMembers
                .Where (member => member != model.UserId)
                .Select (member => new ChatMember () { UserId = member }).ToList ();
            chatMembers.Add (new ChatMember () { UserId = model.UserId });
            var chat = new Chat () {
                AppId = model.AppId,
                Name = model.GroupChatDetail.ChatName,
                ChatType = model.ChatType,
                ChatMembers = chatMembers,
                ChatConversations = new List<ChatConversation> ()
            };
            return await _chatRepository.CreateAsync (chat);
        }

        private bool IsNotValidForCreatingGroupChat (ChatVM model) {
            if (model.GroupChatDetail == null ||
                string.IsNullOrWhiteSpace (model.GroupChatDetail.ChatName) ||
                model.GroupChatDetail.OtherMembers == null) {
                return true;
            }
            return false;
        }

    }
}