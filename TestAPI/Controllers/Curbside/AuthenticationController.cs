using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TestAPI.Extensions;
using TestAPI.Helpers;
using TestAPI.ModelContexts;
using TestAPI.Models.Curbside;

namespace TestAPI.Controllers.Curbside
{
    [ApiController]
    [Route("v1/api/curbside/[controller]")]
    public class AuthenticationController : MyControllerBase<CurbsideContext>
    {
        public AuthenticationController(CurbsideContext context, IWebHostEnvironment environment) : base(context, environment)
        {
        }

        [HttpPut("sign_up")]
        public async Task<object> SignUp()
        {
            Profile userFound = null;
            Logger.Log($"HOST: {Request.Host.Value} | HOST = {Request.Host.Host} | PORT = {Request.Host.Port}");
            if (!Request.HasFormContentType)
            {
                var user = await this.GetRequestRObject<Profile>();
                if (!user.Email.IsValidString())
                {
                    return MessageExtension.ShowRequiredMessage("Email address");
                }
                else if (!user.Email.IsValidEmail())
                {
                    return MessageExtension.ShowCustomMessage("Sign Up Error", "Email address is not valid", "Okay");
                }

                if (!user.FirstName.IsValidString())
                {
                    return MessageExtension.ShowRequiredMessage("Firstname");
                }

                if (!user.LastName.IsValidString())
                {
                    return MessageExtension.ShowRequiredMessage("Lastname");
                }

                if (!user.Password.IsValidString())
                {
                    return MessageExtension.ShowRequiredMessage("Password");
                }

                userFound = await dbContext.Profiles.FirstOrDefaultAsync((userObject) => userObject.Email.Equals(user.Email));
                if (userFound == null)
                {
                    user.Password = SaltHasher.ComputeHash(user.Password);
                    var userAdded = await dbContext.Profiles.AddAsync(user);
                    await dbContext.SaveChangesAsync();
                    return new { user = userAdded.Entity.UserFormat(Request.Host.Value), status = 200 };
                }
                else
                {
                    return MessageExtension.ShowCustomMessage("Sign Up Error", "Email is already taken", "Okay");
                }
            }

            return NotFound();
        }

        [HttpPost("sign_in")]
        public async Task<object> SignIn()
        {
            if (!Request.HasFormContentType)
            {
                var user = await this.GetRequestRObject<Profile>();
                if (!user.Email.IsValidString())
                {
                    return MessageExtension.ShowRequiredMessage("Email");
                }
                else if (!user.Email.IsValidEmail())
                {
                    return MessageExtension.ShowCustomMessage("Sign In Error", "Email address is not valid", "Okay");
                }

                if (!user.Password.IsValidString())
                {
                    return MessageExtension.ShowRequiredMessage("Password");
                }
                Logger.Log($"QUERY");
                Profile userFound = await dbContext.Profiles.FirstOrDefaultAsync((userObject) => userObject.Email.Equals(user.Email));
                if (userFound != null)
                {
                    if (SaltHasher.VerifyHash(user.Password, userFound.Password))
                    {
                        dbContext.Entry(userFound).Reference(u => u.ImageFile).Load(); // retreived data from reference entity (ImageFile Property from ImageID)
                        return new { user = userFound.UserFormat(Request.Host.Value), status = HttpStatusCode.OK };
                    }
                    return MessageExtension.ShowCustomMessage("Sign In Error", "Username or password mismatched", "Sign Up");
                }
                else
                {
                    return MessageExtension.ShowCustomMessage("Sign In Error", "User is not registered", "Sign Up");
                }
            }

            return NotFound();
        }

    }
}
