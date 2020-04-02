using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;

namespace ChatServer.Model.ViewModels
{
    public class UserChatVM
    {
        public string UserId { get; set; }
        public Chat Chat { get; set; }
        public int NewMessagesCount { get; set; }
    }
}
