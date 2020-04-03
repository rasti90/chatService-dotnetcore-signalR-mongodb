using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;
using ChatServer.Model.ViewModels;
using ChatServer.Service;
using ChatServer.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase {
        private readonly IAuthenticationService _athenticationService;

        public AuthenticationController (IAuthenticationService authenticationService) {
            this._athenticationService = authenticationService;
        }

        // GET api/Authentication
        [HttpGet]
        public async Task<ActionResult<List<String>>> Get () {
            // var apps = await _applicationService.GetAsync();
            // return apps.Select(app => app.name).ToList();
            return new List<string> ();
        }

        [AllowAnonymous]
        [HttpPost ("authenticate")]
        public async Task<IActionResult> Authenticate ([FromForm] AuthenticateVM model) {
            var token = await _athenticationService.Authenticate (model);

            if (token == null)
                return BadRequest (new { message = "can not register the user" });

            return Ok (token);
        }

        // [AllowAnonymous]
        // [HttpPost("authenticate")]
        // public async Task<IActionResult> Authenticate([FromBody]Authenticate model)
        // {
        //     var user = await _athenticationService.Authenticate(model.Username, model.Password);

        //     if (user == null)
        //         return BadRequest(new { message = "Username or password is incorrect" });

        //     return Ok(user);
        // }

        // GET api/Authentication/D369EE97CDC040C99D5E2C1998E44B9F/c8a2008e-e3d1-425a-9e03-bbda81831c2e
        [HttpGet ("{APIKey}/{userExternalId}")]
        public async Task<ActionResult<string>> Get (string APIKey, string userExternalId) {
            // var app = await _applicationService.GetByAPIKeyAsync(APIKey);
            // if (app != null)
            // {
            //     // this jwt token should be got from SSO services by sending the userExternalId parametre like this: "Negso.SSO.GetJWTToken(userExternalId);"
            //     var SSO_jwt = _applicationService.generateJWTToken(userExternalId);
            //     if (SSO_jwt != null)
            //     {
            //         return SSO_jwt;
            //     }
            //     return BadRequest(new { message = "Authentication Error" });
            // }
            // return BadRequest(new { message = "APIKey is Invalid" });
            return "";
        }
    }
}