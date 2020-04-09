using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Model;
using ChatServer.Repository.Contract;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ChatServer.Repository {
    public class UserProfileRepository : IUserProfileRepository {
        private readonly IMongoCollection<UserProfile> _userProfiles;
        
        private readonly ILogger _logger;
        public UserProfileRepository (IDatabaseSettings settings, ILogger<UserProfileRepository> logger) {
            var client = new MongoClient (settings.ConnectionString);
            var database = client.GetDatabase (settings.DatabaseName);
            this._logger=logger;
            _userProfiles = database.GetCollection<UserProfile> (settings.UserProfilesCollectionName);
        }

        public List<UserProfile> Get () {
            try {
            return _userProfiles.Find (user => true).ToList ();
            } catch (Exception ex) {
                _logger.LogError (ex, "Get UserProfileRepository Exception");
                return null;
            }
        }
        public async Task<List<UserProfile>> GetAsync () {
            try{
            return await _userProfiles.Find (user => true).ToListAsync ();
            } catch (Exception ex) {
                _logger.LogError (ex, "GetAsync UserProfileRepository Exception");
                return null;
            }
        }

        public UserProfile Get (string id) {
            try {
            return _userProfiles.Find<UserProfile> (user => user.Id == id).FirstOrDefault ();
            } catch (Exception ex) {
                _logger.LogError (ex, "Get UserProfileRepository Exception");
                return null;
            }
        }

        public async Task<UserProfile> GetAsync (string id) {
            try {
            return await _userProfiles.Find<UserProfile> (user => user.Id == id).FirstOrDefaultAsync ();
            } catch (Exception ex) {
                _logger.LogError (ex, "GetAsync UserProfileRepository Exception");
                return null;
            }
        }

        public UserProfile Find (Expression<Func<UserProfile, bool>> expression) {
            try {
            return _userProfiles.Find<UserProfile> (expression).FirstOrDefault ();
            } catch (Exception ex) {
                _logger.LogError (ex, "Find UserProfileRepository Exception");
                return null;
            }
        }

        public async Task<UserProfile> FindAsync (Expression<Func<UserProfile, bool>> expression) {
            try {
            return await _userProfiles.Find<UserProfile> (expression).FirstOrDefaultAsync ();
            } catch (Exception ex) {
                _logger.LogError (ex, "FindAsync UserProfileRepository Exception");
                return null;
            }
        }

        public UserProfile Create (UserProfile user) {
            try {
            _userProfiles.InsertOne (user);
            return user;
            } catch (Exception ex) {
                _logger.LogError (ex, "Create UserProfileRepository Exception");
                return null;
            }
        }

        public async Task<UserProfile> CreateAsync (UserProfile user) {
            try {
            await _userProfiles.InsertOneAsync (user);
            return user;
            } catch (Exception ex) {
                _logger.LogError (ex, "CreateAsync UserProfileRepository Exception");
                return null;
            }
        }

        public void Update (string id, UserProfile userIn) {
            try {
            _userProfiles.ReplaceOne (user => user.Id == id, userIn);
            } catch (Exception ex) {
                _logger.LogError (ex, "Update UserProfileRepository Exception");
            }
        }

        public void Remove (UserProfile userIn) {
            try {
            _userProfiles.DeleteOne (user => user.Id == userIn.Id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove UserProfileRepository Exception");
            }
        }

        public void Remove (string id) {
            try {
            _userProfiles.DeleteOne (user => user.Id == id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove UserProfileRepository Exception");
            }
        }

    }
}