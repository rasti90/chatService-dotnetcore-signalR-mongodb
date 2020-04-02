using MongoDB.Driver;
using ChatServer.Model;
using ChatServer.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Repository.Contract;

namespace ChatServer.Repository
{
    public class UserRepository:IUserRepository
    {
        private readonly IMongoCollection<User> _users;
        public UserRepository(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public List<User> GetByAppId(string appId) =>
            _users.Find(user => user.AppId==appId).Project<User>(Builders<User>.Projection
                .Exclude("Activities").Exclude("Connections")).ToList();

        public async Task<List<User>> GetByAppIdAsync(string appId) =>
            await _users.Find(user => user.AppId==appId).Project<User>(Builders<User>.Projection
                .Exclude("Activities").Exclude("Connections")).ToListAsync();
            

        public User Get(string id) =>
            _users.Find<User>(user => user.Id == id).Project<User>(Builders<User>.Projection
                .Exclude("Activities").Exclude("Connections")).FirstOrDefault();

        public async Task<User> GetAsync(string id) =>
            await _users.Find<User>(user => user.Id == id).Project<User>(Builders<User>.Projection
                .Exclude("Activities").Exclude("Connections")).FirstOrDefaultAsync();

        public User GetByExternalId(string appId, string externalId) =>
            _users.Find<User>(user => user.AppId==appId && user.ExternalId == externalId).Project<User>(Builders<User>.Projection
                .Exclude("Activities").Exclude("Connections")).FirstOrDefault();

        public async Task<User> GetByExternalIdAsync(string appId, string externalId) =>
            await _users.Find<User>(user => user.AppId==appId && user.ExternalId == externalId).Project<User>(Builders<User>.Projection
                .Exclude("Activities").Exclude("Connections")).FirstOrDefaultAsync();

        public User GetByConnectionId(string connectionId) =>
            _users.Find<User>(user => user.Connections.Any(connection => connection.ConnectionId == connectionId)).Project<User>(Builders<User>.Projection
                .Exclude("Activities").Exclude("Connections")).FirstOrDefault();

        public async Task<User> GetByConnectionIdAsync(string connectionId) =>
            await _users.Find<User>(user => user.Connections.Any(connection => connection.ConnectionId == connectionId)).Project<User>(Builders<User>.Projection
                .Exclude("Activities").Exclude("Connections")).FirstOrDefaultAsync();

        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        public async Task<User> CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        public void Update(string id, User userIn) =>
            _users.ReplaceOne(user => user.Id == id, userIn);

        public async Task UpdateAsync(string id, User userIn) =>
           await _users.ReplaceOneAsync(user => user.Id == id, userIn);

        public void AddActivityAndManageConnectionToUser(string userId, Activity activity, Connection connection)
        {
            if (activity != null)
            {
                UpdateDefinition<User> updateQuery = Builders<User>.Update.Push("Activities", activity);
                bool isOnline = activity.ActivityType == ChatServer.Model.Enum.ActivityType.getOnline;
                updateQuery = updateQuery.Set("IsOnline", isOnline);
                updateQuery = (isOnline && connection != null) ? updateQuery.Push("Connections", connection) : updateQuery.Pull("Connections", connection);
                _users.FindOneAndUpdate(Builders<User>.Filter.Eq("Id", userId), updateQuery);
            }
        }

        public async Task AddActivityAndManageConnectionToUserAsync(string userId, Activity activity, Connection connection)
        {          
            try{
            if (activity != null)
            {
                UpdateDefinition<User> updateQuery = Builders<User>.Update.Push("Activities", activity);
                bool isOnline = activity.ActivityType == ChatServer.Model.Enum.ActivityType.getOnline;
                updateQuery = updateQuery.Set("IsOnline", isOnline);
                updateQuery = (isOnline && connection != null) ? updateQuery.Push("Connections", connection) : updateQuery.Pull("Connections", connection);
                await _users.FindOneAndUpdateAsync(Builders<User>.Filter.Eq("Id", userId), updateQuery);
            }
            }catch(Exception ex){
                
            }
        }

        public void Remove(User userIn) =>
            _users.DeleteOne(user => user.Id == userIn.Id);

        public async Task RemoveAsync(User userIn) =>
            await _users.DeleteOneAsync(user => user.Id == userIn.Id);

        public void Remove(string id) =>
            _users.DeleteOne(user => user.Id == id);

        public async Task RemoveAsync(string id) =>
            await _users.DeleteOneAsync(user => user.Id == id);

    }
}
