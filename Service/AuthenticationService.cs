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
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAppSettings _appSettings;

        public AuthenticationService(IUserProfileRepository userProfileRepository,
            IApplicationRepository applicationRepository,
            IUserRepository userRepository,
            IAppSettings appSettings){
            this._userProfileRepository=userProfileRepository;
            this._applicationRepository=applicationRepository;
            this._userRepository=userRepository;
            this._appSettings=appSettings;
        }
        private List<UserProfile> _users = new List<UserProfile>
        { 
            new UserProfile { Id = "1", firstName = "Test", lastName = "User", userName = "test", passwordHash = "test".GetHashCode().ToString() } 
        };

        public async Task<string> Authenticate(AuthenticateVM model)
        {
            var app = await _applicationRepository.GetByAPIKeyAsync(model.APIKey);

            if (app != null)
            {
                var user = await _userRepository.GetByExternalIdAsync(app.Id, model.UserExternalId);
                if (user == null)
                {
                    user = await _userRepository.CreateAsync(new User()
                    {
                        FullName=model.Firstname+ " " + model.Lastname,
                        ExternalId=model.UserExternalId,
                        AppId=app.Id,
                        IsOnline=true,
                        Activities=new List<Activity>(),
                        Connections=new List<Connection>()
                    });
                }
                else
                {                   
                    user.FullName=model.Firstname+ " " + model.Lastname;
                    user.IsOnline=true;
                    await _userRepository.UpdateAsync(user.Id, user);
                }

                var token=getToken(model, app.Id, user.Id);
                return token;
            }
            return null;                   
        }
        // public async Task<UserProfile> Authenticate(string username, string password)
        // {
        //     var key = Encoding.ASCII.GetBytes(_appSettings.secret);
        //     string passwordHash=password.GetCustomHashCode(key);
        //     var user = await _userProfileRepository.FindAsync(u => u.userName == username && u.passwordHash == passwordHash);
        //     //var user = _users.Find(u => u.userName == username && u.passwordHash == passwordHash);

        //     // return null if user not found
        //     if (user == null)
        //         return null;

        //     // authentication successful so generate jwt token
        //     var tokenHandler = new JwtSecurityTokenHandler();   
        //     var tokenDescriptor = new SecurityTokenDescriptor
        //     {
        //         Subject = new ClaimsIdentity(new Claim[] 
        //         {
        //             new Claim(ClaimTypes.Name, user.Id.ToString())
        //         }),
        //         Expires = DateTime.UtcNow.AddDays(7),
        //         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //     };
        //     var token = tokenHandler.CreateToken(tokenDescriptor);
        //     user.token = tokenHandler.WriteToken(token);

        //     return user.WithoutPassword();        
        // }

        private string getToken(AuthenticateVM model, string appId, string userId){
            var key = Encoding.ASCII.GetBytes(_appSettings.secret);

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();   
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    //new Claim(ClaimTypes.Name, model.UserId.ToString())
                    new Claim("UserId", userId),
                    new Claim("UserExternalId", model.UserExternalId.ToString()),
                    new Claim("APIKey", model.APIKey),
                    new Claim("AppId", appId)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}