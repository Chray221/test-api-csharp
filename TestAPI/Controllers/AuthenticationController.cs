using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using TestAPI.Helpers;
using TestAPI.ModelContexts;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [ApiController]
    [Route("v1/api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        // initializing DB
        //TestContext testDB = new TestContext();
        TestContext testDB;

        string RootPath { get { return Host.HostEnvironment.WebRootPath ?? Host.HostEnvironment.ContentRootPath; } }

        public AuthenticationController(TestContext context, IWebHostEnvironment environment)
        {
            Host.HostEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            testDB = context;
            testDB.Database.EnsureCreated();
            Debug.WriteLine($"PATH {RootPath}");
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
                User userFound = await testDB.Users.FirstOrDefaultAsync((userObject) => userObject.Username.Equals(user.Username));
                testDB.Entry(userFound).Reference(u => u.ImageFile).Load(); // retreived data from reference entity (ImageFile Property from ImageID)
                if ( userFound != null)
                {
                    if (SaltHasher.VerifyHash(user.Password, userFound.Password))
                        return new { user = userFound.UserFormat(Request.Host.Value), status = HttpStatusCode.OK };
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
                userFound = await testDB.Users.FirstOrDefaultAsync((userObject) => userObject.Username.Equals(Request.Form["username"].First()));
                if (userFound == null)
                {
                    User user = new User(Request.Form["username"].First(), Request.Form["first_name"].First(), Request.Form["last_name"].First(), SaltHasher.ComputeHash(Request.Form["password"].First()));
                                   
                    if (Request.Form.Files["image"] != null)
                    {
                        IFormFile imageFile = Request.Form.Files["image"];
                        string imageName = imageFile.FileName;

                        //save image with thumb
                        //ImageFile image = ;

                        EntityEntry<ImageFile> userImage = await testDB.Images.AddAsync(new ImageFile($"{Guid.NewGuid()}{Path.GetExtension(imageName)}"));
                        await testDB.SaveChangesAsync();
                        user.ImageId = userImage.Entity.Id;
                        //user.Image = Path.Combine(RootPath, $"images", $"{user.Id}{Path.GetExtension(imageName)}");
                        //user.Image = Path.Combine($"images", $"");
                        Logger.Log($" {Request.Form.FirstOrDefault((requestData) => requestData.Key.Equals("image")).Value}");
                        //testDB.Users.Update(userAdded.Entity);
                        await ImageHelper.SaveImage(imageFile, userImage.Entity.Url);
                        await ImageHelper.SaveImage(imageFile, userImage.Entity.ThumbUrl);
                    }
                    EntityEntry<User> userAdded = await testDB.Users.AddAsync(user);
                    Logger.Log($"ADDED: {userAdded.Entity.Id}");
                    var savedChanges = await testDB.SaveChangesAsync();
                    Logger.Log($"SAVED: {userAdded.Entity.Id} {savedChanges} ");
                    return new { user = userAdded.Entity.UserFormat(Request.Host.Value), status = HttpStatusCode.OK };
                }
                else
                {
                    return MessageExtension.ShowCustomMessage("Sign Up Error", "Username is already taken", "Okay");
                }
            }
            else if ( !Request.HasFormContentType )
            {
                var user = this.GetRequestJObjectAsync();
                //if (userFound == null)
                //{
                //    user.Password = SaltHasher.ComputeHash(user.Password);
                //    var userAdded = await testDB.Users.AddAsync(user);                    
                //    if (Request.Form.ContainsKey("image"))
                //    {
                //        string imageName = "ImageName.jpg";
                //        user.Image = Path.Combine(RootPath, $"user/{userAdded.Entity.Id}",Path.GetExtension(imageName));
                //        Logger.Log($" {Request.Form.FirstOrDefault((requestData) => requestData.Key.Equals("image")).Value}");
                //        //if()
                //        //{

                //        //}
                //        //ImageHelper.SaveImage(null, user.Image);
                //        //Request.Form.FirstOrDefault((obj) => obj.Key.Equals("image")).Value.
                //    }
                //    await testDB.SaveChangesAsync();
                //    return new { user = userAdded.Entity.UserFormat(), status = 200 };
                //}
                //else
                //{
                //    return MessageExtension.ShowCustomMessage("Sign Up Error", "Username is already taken", "Okay");
                //}
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
