using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Model;
using ChatServer.Repository.Contract;
using MongoDB.Driver;

namespace ChatServer.Repository {
    public class UserProfileRepository : IUserProfileRepository {
        private readonly IMongoCollection<UserProfile> _userProfiles;
        public UserProfileRepository (IDatabaseSettings settings) {
            var client = new MongoClient (settings.ConnectionString);
            var database = client.GetDatabase (settings.DatabaseName);

            _userProfiles = database.GetCollection<UserProfile> (settings.UserProfilesCollectionName);
        }

        public List<UserProfile> Get () =>
            _userProfiles.Find (user => true).ToList ();
        public async Task<List<UserProfile>> GetAsync () =>
            await _userProfiles.Find (user => true).ToListAsync ();

        public UserProfile Get (string id) =>
            _userProfiles.Find<UserProfile> (user => user.Id == id).FirstOrDefault ();

        public async Task<UserProfile> GetAsync (string id) =>
            await _userProfiles.Find<UserProfile> (user => user.Id == id).FirstOrDefaultAsync ();

        public UserProfile Find (Expression<Func<UserProfile, bool>> expression) =>
            _userProfiles.Find<UserProfile> (expression).FirstOrDefault ();

        public async Task<UserProfile> FindAsync (Expression<Func<UserProfile, bool>> expression) =>
            await _userProfiles.Find<UserProfile> (expression).FirstOrDefaultAsync ();

        public UserProfile Create (UserProfile user) {
            _userProfiles.InsertOne (user);
            return user;
        }

        public async Task<UserProfile> CreateAsync (UserProfile user) {
            await _userProfiles.InsertOneAsync (user);
            return user;
        }

        public void Update (string id, UserProfile userIn) =>
            _userProfiles.ReplaceOne (user => user.Id == id, userIn);

        public void Remove (UserProfile userIn) =>
            _userProfiles.DeleteOne (user => user.Id == userIn.Id);

        public void Remove (string id) =>
            _userProfiles.DeleteOne (user => user.Id == id);

    }
}