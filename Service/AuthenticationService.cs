using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Model;
using ChatServer.Model.ViewModels;
using ChatServer.Repository.Contract;
using ChatServer.Service.Contract;
using Microsoft.IdentityModel.Tokens;

namespace ChatServer.Service {
    public class AuthenticationService : IAuthenticationService {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAppSettings _appSettings;

        public AuthenticationService (IApplicationRepository applicationRepository,
            IUserRepository userRepository,
            IAppSettings appSettings) {
            this._applicationRepository = applicationRepository;
            this._userRepository = userRepository;
            this._appSettings = appSettings;
        }

        public async Task<string> Authenticate (AuthenticateVM model) {
            var app = await _applicationRepository.GetByAPIKeyAsync (model.APIKey);

            if (app != null) {
                var user = await _userRepository.GetByExternalIdAsync (app.Id, model.UserExternalId);
                if (user == null) {
                    user = await _userRepository.CreateAsync (new User () {
                        FullName = model.Firstname + " " + model.Lastname,
                            ExternalId = model.UserExternalId,
                            AppId = app.Id,
                            IsOnline = false,
                            IsActive = true,
                            Activities = new List<Activity> (),
                            Connections = new List<Connection> ()
                    });
                } else {
                    user.FullName = model.Firstname + " " + model.Lastname;
                    await _userRepository.updateFullNameAsync (user.Id, user.FullName);
                }

                var token = getToken (model, app.Id, user.Id);
                return token;
            }
            return null;
        }

        private string getToken (AuthenticateVM model, string appId, string userId) {
            var key = Encoding.ASCII.GetBytes (_appSettings.secret);

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler ();
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (new Claim[] {
                //new Claim(ClaimTypes.Name, model.UserId.ToString())
                new Claim ("UserId", userId),
                new Claim ("UserExternalId", model.UserExternalId.ToString ()),
                new Claim ("APIKey", model.APIKey),
                new Claim ("AppId", appId)
                }),
                Expires = DateTime.UtcNow.AddDays (7),
                SigningCredentials = new SigningCredentials (new SymmetricSecurityKey (key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken (tokenDescriptor);
            return tokenHandler.WriteToken (token);
        }
    }
}