using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChatServer.Model;
using ChatServer.Model.ViewModels;
using ChatServer.Service;
using ChatServer.Service.Contract;
using Microsoft.AspNetCore.Authorization;

namespace ChatServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            this._registrationService=registrationService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterVM model)
        {
            var user = await _registrationService.Register(model);

            if (user == null)
                return BadRequest(new { message = "The Username is used befor" });

            return Ok(user);
        }
        
       
    }
}