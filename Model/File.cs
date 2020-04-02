using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatServer.Model
{
    public class File
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Filename { get; set; }
        public DateTime UploadDate { get; set; }
        public FileMetadata Metadata { get; set; }
        [BsonRepresentation(BsonType.Binary)]
        public BsonBinaryData Data { get; set; }
    }
}
