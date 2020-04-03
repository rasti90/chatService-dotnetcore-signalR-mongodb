using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Model;
using ChatServer.Repository.Contract;
using MongoDB.Driver;

namespace ChatServer.Repository {
    public class FileRepository : IFileRepository {
        private readonly IMongoCollection<File> _files;
        public FileRepository (IDatabaseSettings settings) {
            var client = new MongoClient (settings.ConnectionString);
            var database = client.GetDatabase (settings.DatabaseName);

            _files = database.GetCollection<File> (settings.FilesCollectionName);
        }

        public List<File> Get () =>
            _files.Find (file => true).ToList ();

        public File Get (string id) =>
            _files.Find<File> (file => file.Id == id).FirstOrDefault ();

        public File Create (File file) {
            _files.InsertOne (file);
            return file;
        }

        public void Update (string id, File fileIn) =>
            _files.ReplaceOne (file => file.Id == id, fileIn);

        public void Remove (File fileIn) =>
            _files.DeleteOne (file => file.Id == fileIn.Id);

        public void Remove (string id) =>
            _files.DeleteOne (file => file.Id == id);

    }
}