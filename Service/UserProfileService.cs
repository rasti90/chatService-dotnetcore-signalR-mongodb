using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;
using ChatServer.Model.ViewModels;
using ChatServer.Repository.Contract;
using ChatServer.Service.Contract;
using MongoDB.Bson;

namespace ChatServer.Service {
    public class UserProfileService : IUserProfileService {
        private readonly IUserProfileRepository _userProfileRepository;
        public UserProfileService (IUserProfileRepository userProfileRepository) {
            this._userProfileRepository = userProfileRepository;
        }
        public async Task<UserProfile> GetUserInformation (string userId) {
            var userProfile = await _userProfileRepository.GetAsync (userId);
            return userProfile;
        }
    }
}