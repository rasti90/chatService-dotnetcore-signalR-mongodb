using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;

namespace ChatServer.Model.ViewModels
{
    public class ChatMembersVM
    {
        public string ChatId { get; set; }
        public List<ChatMember> ChatMembers {get;set;}
    }
}
