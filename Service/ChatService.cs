using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;
using ChatServer.Model.ViewModels;
using ChatServer.Model.Enum;
using ChatServer.Service.Contract;
using ChatServer.Repository.Contract;
using System;
using MongoDB.Bson;

namespace ChatServer.Service
{
    public partial class ChatService : IChatService
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IApplicationRepository _applicationRepository;

        private readonly IUserService _userService;

        public ChatService(IUserRepository userRepository, IChatRepository chatRepository
            , IApplicationRepository applicationRepository, IUserService userService){
            this._userRepository=userRepository;
            this._chatRepository=chatRepository;
            this._applicationRepository=applicationRepository;
            this._userService=userService;
        }

        public async Task<List<ChatMember>> AddMembersToChat(string appId, ChatMembersVM model)
        {
            var app = await _applicationRepository.GetAsync(appId);
            if (app != null)
            {
                var chat = await _chatRepository.GetAsync(app.Id, model.ChatId);
                if (chat != null && chat.ChatType==ChatType.Group)
                {
                    chat.ChatMembers = chat.ChatMembers != null ? chat.ChatMembers : new List<ChatMember>();
                    var shouldBeAdded=model.ChatMembers
                        .Where(member => !chat.ChatMembers.Any(oldMember => oldMember.UserId==member.UserId))
                        .Select(member => new ChatMember(){
                            UserId=member.UserId
                        }).ToList();
                    chat.ChatMembers.AddRange(shouldBeAdded);
                    
                    await _chatRepository.UpdateAsync(chat.Id, chat);
                    return shouldBeAdded;
                }
            }
            return null;
        }

        public async Task<Chat> CreateChat(string appId, string userId, ChatVM model)
        {
            var app = await _applicationRepository.GetAsync(appId);
            if (app != null)
            {
                var user=await _userRepository.GetAsync(userId);
                model.AppId=app.Id;
                model.UserId=user.Id;
                var chat=await CreateChatRecord(model);
                return chat;
            }
            return null;
        }

        public async Task<Chat> GetChatInformation(string appId, string userId, string chatId)
        {
            var app = await _applicationRepository.GetAsync(appId);
            if (app != null)
            {
                var user=await _userRepository.GetAsync(userId);
                var chat = await _chatRepository.GetAsync(app.Id, chatId);
                if(user!=null && chat!=null){
                    if (chat.ChatMembers.Any(member => member.UserId == user.Id)){
                        if(chat.ChatType==ChatType.Private){
                            var otherUser=chat.ChatMembers.Find(chatMember => chatMember.UserId!=userId);
                            var otherUserInfo=await _userRepository.GetAsync(otherUser.UserId);
                            chat.Name=otherUserInfo.FullName;
                        }
                        return chat;
                    }
                }
            }
            return null;
        }

        public async Task<List<ChatMember>> GetChatMembers(string appId, string chatId)
        {
            var app = await _applicationRepository.GetAsync(appId);
            if (app != null)
            {
                var chatMembers = await _chatRepository.GetChatMembersAsync(appId, chatId);
                return chatMembers;
            }
            return null;
        }
    }
}