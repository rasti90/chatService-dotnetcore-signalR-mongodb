using System.Collections.Generic;

namespace ChatServer.Model.ViewModels {
    public class GroupChatDetailVM {
        public string ChatName { get; set; }
        public List<string> OtherMembers { get; set; }
    }
}