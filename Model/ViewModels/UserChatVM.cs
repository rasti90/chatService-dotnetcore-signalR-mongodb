
namespace ChatServer.Model.ViewModels {
    public class UserChatVM {
        public string UserId { get; set; }
        public Chat Chat { get; set; }
        public int NewMessagesCount { get; set; }
    }
}