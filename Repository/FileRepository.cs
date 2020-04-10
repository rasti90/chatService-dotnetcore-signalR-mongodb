using System;
using System.Collections.Generic;
using System.Linq;
using ChatServer.Helper;
using ChatServer.Model;
using ChatServer.Repository.Contract;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ChatServer.Repository {
    public class FileRepository : IFileRepository {
        private readonly IMongoCollection<File> _files;

        private readonly ILogger _logger;
        public FileRepository (IDatabaseSettings settings, ILogger<FileRepository> logger) {
            var client = new MongoClient (settings.ConnectionString);
            var database = client.GetDatabase (settings.DatabaseName);
            this._logger = logger;
            _files = database.GetCollection<File> (settings.FilesCollectionName);
        }

        public List<File> Get () {
            try {
                return _files.Find (file => true).ToList ();
            } catch (Exception ex) {
                _logger.LogError (ex, "Get FileRepository Exception");
                return null;
            }
        }

        public File Get (string id) {
            try {
                return _files.Find<File> (file => file.Id == id).FirstOrDefault ();
            } catch (Exception ex) {
                _logger.LogError (ex, "Get FileRepository Exception");
                return null;
            }
        }

        public File Create (File file) {
            try {
                _files.InsertOne (file);
                return file;
            } catch (Exception ex) {
                _logger.LogError (ex, "InsertOne FileRepository Exception");
                return null;
            }
        }

        public void Update (string id, File fileIn) {
            try {
                _files.ReplaceOne (file => file.Id == id, fileIn);
            } catch (Exception ex) {
                _logger.LogError (ex, "Update FileRepository Exception");
            }
        }

        public void Remove (File fileIn) {
            try {
                _files.DeleteOne (file => file.Id == fileIn.Id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove FileRepository Exception");
            }
        }

        public void Remove (string id) {
            try {
                _files.DeleteOne (file => file.Id == id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove FileRepository Exception");
            }
        }

    }
}