using System.Threading.Tasks;
using ChatServer.Model.ViewModels;
using ChatServer.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase {
        private readonly IAuthenticationService _athenticationService;

        public AuthenticationController (IAuthenticationService authenticationService) {
            this._athenticationService = authenticationService;
        }

        /// <summary>
        /// Authenticate Client User To Use The Chat Server Rest API
        /// </summary>
        /// /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/Authentication/authenticate
        ///     {
        ///        "APIKey": "D369EE97CDC040C99D5E2C1998E44B9F",
        ///        "UserExternalId": "42E6F79B-5649-4FD3-8405-DEE5C7D616E5",
        ///        "Firstname": "Mohammad",
        ///        "Lastname": "Rasti"
        ///     }
        ///
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost ("authenticate")]
        public async Task<IActionResult> AuthenticateUser ([FromForm] AuthenticateVM model) {
            var token = await _athenticationService.Authenticate (model);

            if (token == null)
                return BadRequest (new { message = "can not register the user" });

            return Ok (token);
        }

    }
}