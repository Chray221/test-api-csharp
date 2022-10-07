using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Data;
using TestAPI.Helpers;
using TestAPI.ModelContexts;
using TestAPI.Models;
using TestAPI.Services.Contracts;

namespace TestAPI.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiversion}/api/[controller]")]
    public class UserController : MyControllerBase
    {
        #region fields
        private IUserRepository _userRepository;
        #endregion

        public UserController( IWebHostEnvironment environment
            , IUserRepository userRepository) :base(environment)
        {
            _userRepository = userRepository;
        }

        [HttpGet("{idOrUsername}", Order = 0)]
        public async Task<ActionResult> Get([FromRoute] string idOrUsername)
        {
            if(Guid.TryParse(idOrUsername,out Guid userId))
            {
                User user = await _userRepository.GetAsync(userId);
                if(user != null)
                {
                    return Ok(new PublicProfile(user,this.GetRootUrl()));
                }
            }
            else if (!string.IsNullOrEmpty(idOrUsername))
            {
                User user = await _userRepository.GetAsync(idOrUsername);
                if (user != null)
                {
                    return Ok(new PublicProfile(user, this.GetRootUrl()));
                }
            }
            
            return NotFound(MessageExtension.ShowCustomMessage("Not Found", "User not found"));
        }
    }

}
