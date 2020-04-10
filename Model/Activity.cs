using System;
using ChatServer.Model.Enum;

namespace ChatServer.Model {
    public class Activity {
        public ActivityType ActivityType { get; set; }
        public DateTime Date { get; set; }
        public string ConnectionId { get; set; }
    }
}