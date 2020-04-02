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
    public partial class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IApplicationRepository _applicationRepository;

        public UserService(IUserRepository userRepository, IChatRepository chatRepository
            , IApplicationRepository applicationRepository){
            this._userRepository=userRepository;
            this._chatRepository=chatRepository;
            this._applicationRepository=applicationRepository;
        }

        public async Task<List<UserChatVM>> GetUserChats(string appId, string userId)
        {
            var app = await _applicationRepository.GetAsync(appId);
            if (app != null)
            {
                var user = await _userRepository.GetAsync(userId);
                if (user != null)
                {
                    var chats = await _chatRepository.GetByUserIdAsync(app.Id, user.Id);
                    List<UserChatVM> userChats = new List<UserChatVM>();
    
                    foreach(var chat in chats)
                    {
                        if(chat.ChatType==ChatType.Private){
                            var otherUser=chat.ChatMembers.Find(chatMember => chatMember.UserId!=userId);
                            var otherUserInfo=await _userRepository.GetAsync(otherUser.UserId);
                            chat.Name=otherUserInfo.FullName;
                        }
                        userChats.Add(new UserChatVM() { UserId = user.Id, Chat = chat, NewMessagesCount = chat.ChatConversations.Count });
                    }
                    return userChats;
                }
            }
            return null;
        }
        
    }
}