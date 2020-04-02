using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ChatServer.Model.Enum;

namespace ChatServer.Model
{
    public class Activity
    {
        public ActivityType ActivityType { get; set; }
        public DateTime Date { get; set; }
        public string ConnectionId { get; set; }
    }
}
