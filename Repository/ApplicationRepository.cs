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
    public class ApplicationRepository : IApplicationRepository {
        private readonly IMongoCollection<Application> _applications;
        private readonly ILogger _logger;
        private readonly IAppSettings _appSettings;
        public ApplicationRepository (IDatabaseSettings settings, IAppSettings appSettings, ILogger<ApplicationRepository> logger) {
            this._appSettings = appSettings;
            this._logger = logger;
            var client = new MongoClient (settings.ConnectionString);
            var database = client.GetDatabase (settings.DatabaseName);

            _applications = database.GetCollection<Application> (settings.ApplicationsCollectionName);
        }

        public async Task SeedDataAsync () {
            try {
                var applicationsCount = await _applications.CountDocumentsAsync (app => true);
                if (applicationsCount == 0) {
                    await _applications.InsertOneAsync (new Application () {
                        Name = "Test Client Application",
                            APIKey = "D369EE97CDC040C99D5E2C1998E44B9F"
                    });
                }
            } catch (Exception ex) {
                _logger.LogError (ex, "SeedDataAsync ApplicationRepository Exception");
            }
        }

        public List<Application> Get () {
            try {
                return _applications.Find (application => true).ToList ();
            } catch (Exception ex) {
                _logger.LogError (ex, "Get Application ApplicationRepository Exception");
                return null;
            }
        }

        public async Task<List<Application>> GetAsync () {
            try {
                return await _applications.Find (application => true).ToListAsync ();
            } catch (Exception ex) {
                _logger.LogError (ex, "Get Applications ApplicationRepository Exception");
                return null;
            }
        }

        public Application Get (string id) {
            try {
                return _applications.Find<Application> (application => application.Id == id).FirstOrDefault ();
            } catch (Exception ex) {
                _logger.LogError (ex, "Get Application ApplicationRepository Exception");
                return null;
            }
        }

        public async Task<Application> GetAsync (string id) {
            try {
                return await _applications.Find<Application> (application => application.Id == id).FirstOrDefaultAsync ();
            } catch (Exception ex) {
                _logger.LogError (ex, "Get Application ApplicationRepository Exception");
                return null;
            }
        }

        public Application GetByAPIKey (string APIKey) {
            try {
                return _applications.Find<Application> (application => application.APIKey == APIKey).FirstOrDefault ();
            } catch (Exception ex) {
                _logger.LogError (ex, "GetByAPIKey ApplicationRepository Exception");
                return null;
            }
        }

        public async Task<Application> GetByAPIKeyAsync (string APIKey) {
            try {
                return await _applications.Find<Application> (application => application.APIKey == APIKey).FirstOrDefaultAsync ();
            } catch (Exception ex) {
                _logger.LogError (ex, "GetByAPIKey ApplicationRepository Exception");
                return null;
            }
        }

        public Application Create (Application application) {
            try {
                _applications.InsertOne (application);
                return application;
            } catch (Exception ex) {
                _logger.LogError (ex, "Create ApplicationRepository Exception");
                return null;
            }
        }

        public async Task<Application> CreateAsync (Application application) {
            try {
                await _applications.InsertOneAsync (application);
                return application;
            } catch (Exception ex) {
                _logger.LogError (ex, "Create ApplicationRepository Exception");
                return null;
            }
        }

        public void Update (string id, Application applicationIn) {
            try {
                _applications.ReplaceOne (application => application.Id == id, applicationIn);
            } catch (Exception ex) {
                _logger.LogError (ex, "Update ApplicationRepository Exception");
            }
        }

        public async Task UpdateAsync (string id, Application applicationIn) {
            try {
                await _applications.ReplaceOneAsync (application => application.Id == id, applicationIn);
            } catch (Exception ex) {
                _logger.LogError (ex, "Update ApplicationRepository Exception");
            }
        }

        public void Remove (Application applicationIn) {
            try {
                _applications.DeleteOne (application => application.Id == applicationIn.Id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove ApplicationRepository Exception");
            }
        }

        public async Task RemoveAsync (Application applicationIn) {
            try {
                await _applications.DeleteOneAsync (application => application.Id == applicationIn.Id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove ApplicationRepository Exception");
            }
        }

        public void Remove (string id) {
            try {
                _applications.DeleteOne (application => application.Id == id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove ApplicationRepository Exception");
            }
        }

        public async Task RemoveAsync (string id) {
            try {
                await _applications.DeleteOneAsync (application => application.Id == id);
            } catch (Exception ex) {
                _logger.LogError (ex, "Remove ApplicationRepository Exception");
            }
        }

    }
}