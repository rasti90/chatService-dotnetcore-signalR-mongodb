using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace ChatServer.Model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ExternalId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string AppId { get; set; }
        public string FullName{get;set;}
        public bool IsActive { get; set; }
        public bool IsOnline { get; set; }
        public List<Connection> Connections { get; set; }
        public List<Activity> Activities { get; set; }
        [BsonIgnore]
        public Application Application{get;set;}
    }
}
