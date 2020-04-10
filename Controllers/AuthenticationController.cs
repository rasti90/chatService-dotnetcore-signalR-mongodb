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