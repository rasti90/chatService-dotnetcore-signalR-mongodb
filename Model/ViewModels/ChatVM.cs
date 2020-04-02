using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ChatServer.Model.Enum;

namespace ChatServer.Model.ViewModels
{
    public class ChatVM
    {
        public ChatType ChatType{get;set;}  
        public PrivateChatDetailVM PrivateChatDetail{get;set;}
        public GroupChatDetailVM GroupChatDetail{get;set;} 
        public string AppId{get;set;}
        public string UserId{get;set;}
    }
}