using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ChatServer.Model.Enum;

namespace ChatServer.Model
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string AppId { get; set; }
        public ChatType ChatType { get; set; }
        public List<ChatMember> ChatMembers { get; set; }
        public List<ChatConversation> ChatConversations { get; set; }
        [BsonIgnore]
        public Application Application{get;set;}

    }
}
