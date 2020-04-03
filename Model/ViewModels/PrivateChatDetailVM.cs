using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ChatServer.Model.Enum;

namespace ChatServer.Model.ViewModels {
    public class PrivateChatDetailVM {
        public string OtherMember { get; set; }
    }
}