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
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
    public class AuthenticationController : MyControllerBase<TestContext>
    {
        public AuthenticationController(TestContext context, IWebHostEnvironment environment):base(context, environment)
        {
        }

        // GET: api/authentication
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
                User userFound = await dbContext.Users.FirstOrDefaultAsync((userObject) => userObject.Username.Equals(user.Username));
                if ( userFound != null)
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
                userFound = await dbContext.Users.FirstOrDefaultAsync((userObject) => userObject.Username.Equals(Request.Form["username"].First()));
                if (userFound == null)
                {
                    // create User Temp from HTTPReuestForm
                    User user = new User(Request.Form["username"].First(), Request.Form["first_name"].First(), Request.Form["last_name"].First(), SaltHasher.ComputeHash(Request.Form["password"].First()));
                    IFormFile imageFile = null;
                    // check if image is found in HTTPReuestForm
                    if (Request.Form.Files["image"] is IFormFile imageFormFile)
                    {
                        imageFile = imageFormFile;
                        string imageName = imageFile.FileName;
                        //create and save temporarily ImageFile Temp
                        EntityEntry<ImageFile> userImage = await dbContext.Images.AddAsync(new ImageFile($"{Guid.NewGuid()}{Path.GetExtension(imageName)}"));
                        //save ImageFile Temp in database where userImage is updated with it's id created
                        await dbContext.SaveChangesAsync();
                        // set the User Temp's Id to the created ImageFile
                        user.ImageId = userImage.Entity.Id;
                    }
                    // create and save temporarily the user temp to the database
                    EntityEntry<User> userAdded = await dbContext.Users.AddAsync(user);
                    // save the temporary save user
                    var savedChanges = await dbContext.SaveChangesAsync();
                    if (imageFile != null)
                    {
                        var image = userAdded.Entity.ImageFile;
                        image.Update(userAdded.Entity.Id);
                        ImageHelper.SaveThumbImage(imageFile, image.ThumbUrl);
                        await ImageHelper.SaveImage(imageFile, image.Url);
                        dbContext.Images.Update(image);
                        await dbContext.SaveChangesAsync();
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
                userFound = await dbContext.Users.FirstOrDefaultAsync((userObject) => userObject.Username.Equals(user.Username.First()));
                if (userFound == null)
                {
                    user.Password = SaltHasher.ComputeHash(user.Password);
                    var userAdded = await dbContext.Users.AddAsync(user);
                    await dbContext.SaveChangesAsync();
                    return new { user = userAdded.Entity.UserFormat(Request.Host.Value), status = 200 };
                }
                else
                {
                    return MessageExtension.ShowCustomMessage("Sign Up Error", "Username is already taken", "Okay");
                }
            }

            return NotFound();
        }

        public override BadRequestResult BadRequest()
        {
            return base.BadRequest();
        }

        public override BadRequestObjectResult BadRequest([ActionResultObjectValue] ModelStateDictionary modelState)
        {
            return base.BadRequest(modelState);
        }

        public override BadRequestObjectResult BadRequest([ActionResultObjectValue] object error)
        {
            return base.BadRequest(error);
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
