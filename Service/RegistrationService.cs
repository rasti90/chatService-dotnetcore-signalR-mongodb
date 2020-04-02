using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;
using ChatServer.Helper;
using ChatServer.Model.ViewModels;
using ChatServer.Service.Contract;
using ChatServer.Repository.Contract;
using System;
using MongoDB.Bson;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ChatServer.Service
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IAppSettings _appSettings;

        public RegistrationService(IUserProfileRepository userProfileRepository,
            IAppSettings appSettings){
            this._userProfileRepository=userProfileRepository;
            this._appSettings=appSettings;
        }

        public async Task<UserProfile> Register(RegisterVM model)
        {
            var key = Encoding.ASCII.GetBytes(_appSettings.secret);
            var similarUser=await _userProfileRepository.FindAsync(u => u.userName==model.Username);
            if(similarUser==null){
                UserProfile user=new UserProfile(){
                    firstName=model.Firstname,
                    lastName=model.Lastname,
                    userName=model.Username,
                    passwordHash=model.Password.GetCustomHashCode(key),
                    isActive=true
                };
                user = await _userProfileRepository.CreateAsync(user);
                return user.WithoutPassword();
            }
            return null;
        }
    }
}