using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Model;
using ChatServer.Repository.Contract;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ChatServer.Repository {
    public class UserRepository : IUserRepository {
        private readonly IMongoCollection<User> _users;
         private readonly ILogger _logger;
        public UserRepository (IDatabaseSettings settings, ILogger<UserRepository> logger) {
            var client = new MongoClient (settings.ConnectionString);
            var database = client.GetDatabase (settings.DatabaseName);
            this._logger=logger;
            _users = database.GetCollection<User> (settings.UsersCollectionName);
        }

        public List<User> GetByAppId (string appId) 
        {
            try {
            return _users.Find (user => user.AppId == appId).Project<User> (Builders<User>.Projection
                .Exclude ("Activities").Exclude ("Connections")).ToList ();
                } catch (Exception ex) {
                _logger.LogError (ex, "GetByAppId UserRepository Exception");
                return null;
            }
        }

        public async Task<List<User>> GetByAppIdAsync (string appId) {
            try {
            return await _users.Find (user => user.AppId == appId).Project<User> (Builders<User>.Projection
                .Exclude ("Activities").Exclude ("Connections")).ToListAsync ();
                } catch (Exception ex) {
                _logger.LogError (ex, "GetByAppIdAsync UserRepository Exception");
                return null;
            }
        }

        public User Get (string appId, string userId) {
            try {
            return _users.Find<User> (user => user.AppId == appId && user.Id == userId).Project<User> (Builders<User>.Projection
                .Exclude ("Activities").Exclude ("Connections")).FirstOrDefault ();
                } catch (Exception ex) {
                _logger.LogError (ex, "Get UserRepository Exception");
                return null;
            }
        }

        public async Task<User> GetAsync (string appId, string userId) {
            try {
            return await _users.Find<User> (user => user.AppId == appId && user.Id == userId).Project<User> (Builders<User>.Projection
                .Exclude ("Activities").Exclude ("Connections")).FirstOrDefaultAsync ();
                } catch (Exception ex) {
                _logger.LogError (ex, "GetAsync UserRepository Exception");
                return null;
            }
        }

        public User GetByExternalId (string appId, string externalId) {
            try {
            return _users.Find<User> (user => user.AppId == appId && user.ExternalId == externalId).Project<User> (Builders<User>.Projection
                .Exclude ("Activities").Exclude ("Connections")).FirstOrDefault ();
                } catch (Exception ex) {
                _logger.LogError (ex, "GetByExternalId UserRepository Exception");
                return null;
            }
        }

        public async Task<User> GetByExternalIdAsync (string appId, string externalId) {
            try {
            return await _users.Find<User> (user => user.AppId == appId && user.ExternalId == externalId).Project<User> (Builders<User>.Projection
                .Exclude ("Activities").Exclude ("Connections")).FirstOrDefaultAsync ();
                } catch (Exception ex) {
                _logger.LogError (ex, "GetByExternalIdAsync UserRepository Exception");
                return null;
            }
        }

        public User GetByConnectionId (string connectionId) {
            try {
            return _users.Find<User> (user => user.Connections.Any (connection => connection.ConnectionId == connectionId)).Project<User> (Builders<User>.Projection
                .Exclude ("Activities").Exclude ("Connections")).FirstOrDefault ();
                } catch (Exception ex) {
                _logger.LogError (ex, "GetByConnectionId UserRepository Exception");
                return null;
            }
        }

        public async Task<User> GetByConnectionIdAsync (string connectionId) {
            try {
            return await _users.Find<User> (user => user.Connections.Any (connection => connection.ConnectionId == connectionId)).Project<User> (Builders<User>.Projection
                .Exclude ("Activities").Exclude ("Connections")).FirstOrDefaultAsync ();
                } catch (Exception ex) {
                _logger.LogError (ex, "GetByConnectionIdAsync UserRepository Exception");
                return null;
            }
        }

        public async Task<Connection> GetUserConnectionAsync (string userId, string connectionId) {
            try {
            var user = await _users.Find<User>(user => user.Id == userId).FirstOrDefaultAsync();
            if(user !=null){
                return user.Connections.Find(conn => conn.ConnectionId==connectionId);
            }
            return null;
                } catch (Exception ex) {
                _logger.LogError (ex, "GetUserConnectionAsync UserRepository Exception");
                return null;
            }
        }

        public User Create (User user) {
            try {
            _users.InsertOne (user);
            return user;
            } catch (Exception ex) {
                _logger.LogError (ex, "Create UserRepository Exception");
                return null;
            }
        }

        public async Task<User> CreateAsync (User user) {
            try {
            await _users.InsertOneAsync (user);
            return user;
            } catch (Exception ex) {
                _logger.LogError (ex, "CreateAsync UserRepository Exception");
                return null;
            }
        }

        public void Update (string id, User userIn) {
            try {
            _users.ReplaceOne (user => user.Id == id, userIn);
            } catch (Exception ex) {
                _logger.LogError (ex, "Update UserRepository Exception");
            }
        }

        public async Task UpdateAsync (string id, User userIn) {
            try {
            await _users.ReplaceOneAsync (user => user.Id == id, userIn);
            } catch (Exception ex) {
                _logger.LogError (ex, "UpdateAsync UserRepository Exception");
            }
        }

        public void AddActivityAndManageConnectionToUser (string userId, Activity activity, Connection connection) {
            try {
            if (activity != null) {
                UpdateDefinition<User> updateQuery = Builders<User>.Update.Push ("Activities", activity);
                bool isOnline = activity.ActivityType == ChatServer.Model.Enum.ActivityType.getOnline;
                updateQuery = updateQuery.Set ("IsOnline", isOnline);
                updateQuery = (isOnline && connection != null) ? updateQuery.Push ("Connections", connection) : updateQuery.Pull ("Connections", connection);
                _users.FindOneAndUpdate (Builders<User>.Filter.Eq ("Id", userId), updateQuery);
            }
            } catch (Exception ex) {
                _logger.LogError (ex, "AddActivityAndManageConnectionToUser UserRepository Exception");
            }
        }

        public async Task AddActivityAndManageConnectionToUserAsync (string userId, Activity activity, Connection connection) {
            try {
                if (activity != null) {
                    UpdateDefinition<User> updateQuery = Builders<User>.Update.Push ("Activities", activity);
                    bool isOnline = activity.ActivityType == ChatServer.Model.Enum.ActivityType.getOnline;
                    updateQuery = updateQuery.Set ("IsOnline", isOnline);
                    if(connection!=null){
                        updateQuery = (isOnline) ? updateQuery.Push ("Connections", connection) : updateQuery.Pull ("Connections", connection);
                    }
                    await _users.FindOneAndUpdateAsync (Builders<User>.Filter.Eq ("Id", userId), updateQuery);
                }
            } catch (Exception ex) {
                _logger.LogError (ex, "AddActivityAndManageConnectionToUserAsync UserRepository Exception");
            }
        }

        public void Remove (User userIn) {
            try {
            _users.DeleteOne (user => user.Id == userIn.Id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove UserRepository Exception");
            }
        }

        public async Task RemoveAsync (User userIn) {
            try {
            await _users.DeleteOneAsync (user => user.Id == userIn.Id);
            } catch (Exception ex) {
                _logger.LogError (ex, "RemoveAsync UserRepository Exception");
            }
        }

        public void Remove (string id) {
            try {
            _users.DeleteOne (user => user.Id == id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove UserRepository Exception");
            }
        }

        public async Task RemoveAsync (string id) {
            try {
            await _users.DeleteOneAsync (user => user.Id == id);
            } catch (Exception ex) {
                _logger.LogError (ex, "RemoveAsync UserRepository Exception");
            }
        }

    }
}