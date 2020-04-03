using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatServer.Model {
    public class Connection {
        public string ConnectionId { get; set; }

        public string JWTToken { get; set; }
    }
}