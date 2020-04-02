using MongoDB.Driver;
using ChatServer.Model;
using ChatServer.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChatServer.Repository.Contract;

namespace ChatServer.Repository
{
    public class ApplicationRepository: IApplicationRepository
    {
        private readonly IMongoCollection<Application> _applications;
        private readonly IAppSettings _appSettings;
        public ApplicationRepository(IDatabaseSettings settings, IAppSettings appSettings)
        {
            this._appSettings = appSettings;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _applications = database.GetCollection<Application>(settings.ApplicationsCollectionName);
        }

        public List<Application> Get() =>
            _applications.Find(application => true).ToList();

        public async Task<List<Application>> GetAsync() =>
            await _applications.Find(application => true).ToListAsync();

        public Application Get(string id) =>
            _applications.Find<Application>(application => application.Id == id).FirstOrDefault();

        public async  Task<Application> GetAsync(string id) =>
            await _applications.Find<Application>(application => application.Id == id).FirstOrDefaultAsync();

        public Application GetByAPIKey(string APIKey) =>
            _applications.Find<Application>(application => application.APIKey == APIKey).FirstOrDefault();

        public async Task<Application> GetByAPIKeyAsync(string APIKey) =>
            await _applications.Find<Application>(application => application.APIKey ==APIKey).FirstOrDefaultAsync();

        public Application Create(Application application)
        {
            _applications.InsertOne(application);
            return application;
        }

        public async Task<Application> CreateAsync(Application application)
        {
            await _applications.InsertOneAsync(application);
            return application;
        }

        public void Update(string id, Application applicationIn) =>
            _applications.ReplaceOne(application => application.Id == id, applicationIn);

        public async Task UpdateAsync(string id, Application applicationIn) =>
            await _applications.ReplaceOneAsync(application => application.Id == id, applicationIn);

        public void Remove(Application applicationIn) =>
            _applications.DeleteOne(application => application.Id == applicationIn.Id);

        public async Task RemoveAsync(Application applicationIn) =>
            await _applications.DeleteOneAsync(application => application.Id == applicationIn.Id);

        public void Remove(string id) =>
            _applications.DeleteOne(application => application.Id == id);

        public async Task RemoveAsync(string id) =>
            await _applications.DeleteOneAsync(application => application.Id == id);


        public string generateJWTToken(string userExternalId)
        {
            // // authentication successful so generate jwt token
            // var tokenHandler = new JwtSecurityTokenHandler();
            // var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            // var tokenDescriptor = new SecurityTokenDescriptor
            // {
            //     Subject = new ClaimsIdentity(new Claim[]
            //     {
            //         new Claim(ClaimTypes.Name, userExternalId.ToString())
            //     }),
            //     Expires = DateTime.UtcNow.AddDays(7),
            //     SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            // };
            // var token = tokenHandler.CreateToken(tokenDescriptor);
            // return tokenHandler.WriteToken(token);
            return "";  
        }
    }
}
