using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Model;
using ChatServer.Model.ViewModels;
using ChatServer.Service;
using ChatServer.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers {
    [Authorize]
    [Route ("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly IUserService _userService;

        public UserController (IUserService userService) {
            this._userService = userService;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Get () {
            try {
                var user = User as ClaimsPrincipal;
                string appId = user.GetClaimValue ("AppId");

                var userChats = await _userService.GetUsers (appId);
                return userChats;
            } catch {

            }
            return NotFound ();
        }

        // GET: api/User/5d41494f86f61d731f895f36
        [HttpGet ("{userId}")]
        public async Task<ActionResult<User>> Get (string userId) {
            try {
                var user = User as ClaimsPrincipal;
                string appId = user.GetClaimValue ("AppId");

                var userInfo = await _userService.GetUserInformation (appId, userId);
                if (userInfo != null) {
                    return userInfo;
                }
            } catch {

            }
            return NotFound ();
        }

    }
}