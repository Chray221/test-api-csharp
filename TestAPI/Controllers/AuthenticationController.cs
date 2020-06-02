using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using TestAPI.Helpers;
using TestAPI.ModelContexts;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [ApiController]
    [Route("v1/api/[controller]")]
    public class AuthenticationController : MyControllerBase
    {
        public AuthenticationController(TestContext context, IWebHostEnvironment environment):base(context, environment)
        {
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/authentication/sign_in
        [HttpPost("sign_in")]
        public async Task<object> SignIn(User user)
        {
            if (user != null)
            {
                if (string.IsNullOrEmpty(user.Username))
                {
                    return MessageExtension.ShowRequiredMessage("Username");
                }
                if (string.IsNullOrEmpty(user.Password))
                {
                    return MessageExtension.ShowRequiredMessage("Password");
                }
                Logger.Log($"QUERY");
                User userFound = await testContext.Users.FirstOrDefaultAsync((userObject) => userObject.Username.Equals(user.Username));
                if ( userFound != null)
                {
                    if (SaltHasher.VerifyHash(user.Password, userFound.Password))
                    {
                        testContext.Entry(userFound).Reference(u => u.ImageFile).Load(); // retreived data from reference entity (ImageFile Property from ImageID)
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

        // POST api/authentication/sign_in
        [HttpPut("sign_up")]
        public async Task<object> SignUp()
        {
            User userFound = null;
            Logger.Log($"HOST: {Request.Host.Value} | HOST = {Request.Host.Host} | PORT = {Request.Host.Port}");
            if (Request.HasFormContentType && Request.Form != null)
            {
                if ( !Request.Form.ContainsKey("username") && string.IsNullOrEmpty(Request.Form["username"].First()) )
                {
                    return MessageExtension.ShowRequiredMessage("Username");
                }

                if (!Request.Form.ContainsKey("first_name") && string.IsNullOrEmpty(Request.Form["first_name"].First()))
                {
                    return MessageExtension.ShowRequiredMessage("Firstname");
                }

                if (!Request.Form.ContainsKey("last_name") && string.IsNullOrEmpty(Request.Form["last_name"].First()))
                {
                    return MessageExtension.ShowRequiredMessage("Lastname");
                }

                if (!Request.Form.ContainsKey("password") && string.IsNullOrEmpty(Request.Form["password"].First()))
                {
                    return MessageExtension.ShowRequiredMessage("Password");
                }

                Logger.Log($"QUERY");
                userFound = await testContext.Users.FirstOrDefaultAsync((userObject) => userObject.Username.Equals(Request.Form["username"].First()));
                if (userFound == null)
                {
                    User user = new User(Request.Form["username"].First(), Request.Form["first_name"].First(), Request.Form["last_name"].First(), SaltHasher.ComputeHash(Request.Form["password"].First()));
                    IFormFile imageFile = null;
                    if (Request.Form.Files["image"] is IFormFile imageFormFile)
                    {
                        imageFile = imageFormFile;
                        string imageName = imageFile.FileName;

                        EntityEntry<ImageFile> userImage = await testContext.Images.AddAsync(new ImageFile($"{Guid.NewGuid()}{Path.GetExtension(imageName)}"));
                        await testContext.SaveChangesAsync();
                        user.ImageId = userImage.Entity.Id;
                    }
                    EntityEntry<User> userAdded = await testContext.Users.AddAsync(user);
                    var savedChanges = await testContext.SaveChangesAsync();
                    if (imageFile != null)
                    {
                        var image = userAdded.Entity.ImageFile;
                        image.Update(userAdded.Entity.Id);
                        ImageHelper.SaveThumbImage(imageFile, image.ThumbUrl);
                        await ImageHelper.SaveImage(imageFile, image.Url);
                        testContext.Images.Update(image);
                        await testContext.SaveChangesAsync();
                    }
                    return new { user = userAdded.Entity.UserFormat(Request.Host.Value), status = HttpStatusCode.OK };
                }
                else
                {
                    return MessageExtension.ShowCustomMessage("Sign Up Error", "Username is already taken", "Okay");
                }
            }
            else if ( !Request.HasFormContentType )
            {
                var user = await this.GetRequestRObject<User>();
                if (!user.Username.IsValidString())
                {
                    return MessageExtension.ShowRequiredMessage("Username");
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
                userFound = await testContext.Users.FirstOrDefaultAsync((userObject) => userObject.Username.Equals(user.Username.First()));
                if (userFound == null)
                {
                    user.Password = SaltHasher.ComputeHash(user.Password);
                    var userAdded = await testContext.Users.AddAsync(user);
                    await testContext.SaveChangesAsync();
                    return new { user = userAdded.Entity.UserFormat(), status = 200 };
                }
                else
                {
                    return MessageExtension.ShowCustomMessage("Sign Up Error", "Username is already taken", "Okay");
                }
            }

            return NotFound();
        }

        // POST api/authentication/sign_in
        //[HttpPut("sign_up")]
        //public async Task<object> SignUp(User user)
        //{
        //    if (user != null)
        //    {
        //        if (string.IsNullOrEmpty(user.Username) )
        //        {
        //            return MessageExtension.ShowRequiredMessage("Username");
        //        }

        //        if (string.IsNullOrEmpty(user.FirstName))
        //        {
        //            return MessageExtension.ShowRequiredMessage("Firstname");
        //        }

        //        if (string.IsNullOrEmpty(user.LastName))
        //        {
        //            return MessageExtension.ShowRequiredMessage("Lastname");
        //        }

        //        if (string.IsNullOrEmpty(user.Password))
        //        {
        //            return MessageExtension.ShowRequiredMessage("Password");
        //        }
        //        Logger.Log($"QUERY");
        //        User userFound = await testDB.Users.FirstOrDefaultAsync((userObject) => userObject.Username.Equals(user.Username));
        //        if (userFound == null)
        //        {
        //            user.Password = SaltHasher.ComputeHash(user.Password);
        //            var userAdded = await testDB.Users.AddAsync(user);
        //            await testDB.SaveChangesAsync();
        //            return new { user = userAdded.Entity.UserFormat(), status = 200 };
        //        }
        //        else
        //        {
        //            return MessageExtension.ShowCustomMessage("Sign Up Error", "Username is already taken", "Okay");
        //        }

        //    }

        //    return NotFound();
        //}
    }
}
