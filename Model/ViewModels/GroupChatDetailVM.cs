using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ChatServer.Model.Enum;

namespace ChatServer.Model.ViewModels
{
    public class GroupChatDetailVM
    { 
        public string ChatName{get;set;}
        public List<string> OtherMembers{get;set;} 
    }
}