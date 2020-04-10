using System.Collections.Generic;
using ChatServer.Model.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatServer.Model {
    public class Chat {
        [BsonId]
        [BsonRepresentation (BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }

        [BsonRepresentation (BsonType.ObjectId)]
        public string AppId { get; set; }
        public ChatType ChatType { get; set; }
        public List<ChatMember> ChatMembers { get; set; }
        public List<ChatConversation> ChatConversations { get; set; }

        [BsonIgnore]
        public Application Application { get; set; }

    }
}