using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public AuthenticationController(TestContext context)
        {
            testDB = context;
            //if (testDB.Users == null)
            //    testDB.Users;
            //if (testDB.Users.Count() == 0)
            //{
            //    // Create a new TodoItem if collection is empty,
            //    // which means you can't delete all TodoItems.
            //    //testDB.AddAsync<>
            //    //testDB.Users.Add(new user { Name = "Item1" });
            //    //testDB.SaveChanges();
            //}
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/authentication/sign_in
        [HttpPost("sign_in")]
        public async Task<object> SignIn()
        {
            var signInRequest = await this.GetRequestJObjectAsync();
            if (signInRequest != null)
            {
                if (!signInRequest.ContainsKey("username") || string.IsNullOrEmpty(signInRequest["username"].ToString()))
                {
                    return MessageExtension.ShowRequiredMessage("Username");
                }
                if (!signInRequest.ContainsKey("password") || string.IsNullOrEmpty(signInRequest["password"].ToString()))
                {
                    return  MessageExtension.ShowRequiredMessage("Password");
                }
                User userFound = testDB.Users?.Find(signInRequest["username"].ToStringOrEmpty());
                if ( userFound != null && userFound.Password.Equals(signInRequest["password"] ?? "") )
                {
                    return userFound;
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
        public async Task<object> SignUp(User user)
        {
            if (user != null)
            {
                if ( string.IsNullOrEmpty(user.Username) )
                {
                    return MessageExtension.ShowRequiredMessage("Username");
                }
                if (string.IsNullOrEmpty(user.FirstName) )
                {
                    return MessageExtension.ShowRequiredMessage("FirstName");
                }
                if (string.IsNullOrEmpty(user.LastName))
                {
                    return MessageExtension.ShowRequiredMessage("Lastname");
                }
                if (string.IsNullOrEmpty(user.Password) )
                {
                    return MessageExtension.ShowRequiredMessage("Password");
                }
                User userFound = await testDB.Users.FirstOrDefaultAsync((userObject) => userObject.Username.Equals(user.Username));
                if (userFound == null)
                {
                    var userAdded = testDB.Users.Add(user);
                    await testDB.SaveChangesAsync();
                    return new { user = userAdded, status = 200 };
                }
                else
                {
                    return MessageExtension.ShowCustomMessage("Sign Up Error", "Username is already taken", "Okay");
                }
            }

            return NotFound();
        }
    }
}
