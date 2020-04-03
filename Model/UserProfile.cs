using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatServer.Model {
    public class UserProfile {
        [BsonId]
        [BsonRepresentation (BsonType.ObjectId)]
        public string Id { get; set; }
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string passwordHash { get; set; }
        public string token { get; set; }
        public bool isActive { get; set; }
        public bool isOnline { get; set; }
        public List<Activity> activities { get; set; }

        [BsonIgnore]
        public string fullName {
            get {
                return this.firstName + " " + this.lastName;
            }
            set {

            }
        }
    }
}