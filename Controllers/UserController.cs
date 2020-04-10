using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatServer.Helper;
using ChatServer.Model;
using ChatServer.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers {
    [Authorize]
    [Route ("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly IUserService _userService;

        public UsersController (IUserService userService) {
            this._userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetApplicationUsers () {
            try {
                var user = User as ClaimsPrincipal;
                string appId = user.GetClaimValue ("AppId");

                var userChats = await _userService.GetUsers (appId);
                return userChats;
            } catch {

            }
            return NotFound ();
        }

        // GET: api/users/5d41494f86f61d731f895f36
        [HttpGet ("{userId}")]
        public async Task<ActionResult<User>> GetUser (string userId) {
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