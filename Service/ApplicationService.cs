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
    public class ApplicationService : IApplicationService {
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationService (IApplicationRepository applicationRepository) {
            this._applicationRepository = applicationRepository;
        }

        public async Task SeedData () {
            await _applicationRepository.SeedDataAsync ();
        }

    }
}