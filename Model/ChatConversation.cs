using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatServer.Model
{
    public class ChatConversation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        public string Text { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string FileId { get; set; }
        public DateTime Date { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string ParentConversationId { get; set; }
        public List<ChatConversationReader> ChatConversationReaders { get; set; }
        [BsonIgnore]
        public User User { get; set; }


    }
}
