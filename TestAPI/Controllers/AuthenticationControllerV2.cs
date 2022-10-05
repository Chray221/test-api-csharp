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
using TestAPI.Data;
using TestAPI.Models;
using TestAPI.Services.Contracts;

namespace TestAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiversion}/api/[controller]")]
    public partial class AuthenticationController : MyControllerBase
    {
        private IUserRepository _userRepository;
        private IImageFileRepository _imageFileRepository;
        private TestDbContext _dbContext;
        public AuthenticationController(
            TestDbContext dbContext,
            IWebHostEnvironment environment,
            IUserRepository userRepository,
            IImageFileRepository imageFileRepository)
            :base(environment)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _imageFileRepository = imageFileRepository;
        }

        // POST api/authentication/sign_in
        [HttpPost("sign_in")]
        public async Task<ActionResult> SignIn([FromBody] SignInUserRequestDto user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (user != null)
                    {
                        User userFound = await _userRepository.GetAsync(user.Username);
                        if (userFound != null)
                        {
                            if (SaltHasher.VerifyHash(user.Password, userFound.Password))
                            {
                                _dbContext.Entry(userFound).Reference(u => u.ImageFile).Load(); // retreived data from reference entity (ImageFile Property from ImageID)
                                return Ok(new { user = new UserDto(userFound, RootPath), status = HttpStatusCode.OK });
                            }
                            return BadRequest(MessageExtension.ShowCustomMessage("Sign In Error", "Username or password mismatched", "Sign Up", statusCode: HttpStatusCode.BadRequest));
                        }
                        else
                        {
                            return BadRequest(MessageExtension.ShowCustomMessage("Sign In Error", "User is not registered", "Sign Up", statusCode: HttpStatusCode.BadRequest));
                        }
                    }
                    return NotFound();
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        //POST api/authentication/sign_up
        [HttpPut("sign_up")]
        [Consumes("application/json")]
        public async Task<ActionResult> SignUp([FromBody]SignUpUserRequestDto user)
        {
            if (ModelState.IsValid)
            {
                Logger.Log($"HOST: {Request.Host.Value} | HOST = {Request.Host.Host} | PORT = {Request.Host.Port}");
                if (user != null)
                {
                    if (await _userRepository.GetAsync(user.Username) == null)
                    {
                        User newUser = new User(user.Username, user.FirstName, user.LastName, SaltHasher.ComputeHash(user.Password));
                        if (await _userRepository.InsertAsync(newUser))
                        {
                            return Ok(new UserDto(newUser, RootPath));
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError);
                        }
                    }
                    else
                    {
                        return BadRequest(MessageExtension.ShowCustomMessage("Sing Up Error!", "User already exists", statusCode: HttpStatusCode.BadRequest));
                    }
                }

                return BadRequest();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("sign_up")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> SignUpForm([FromForm] SignUpUserRequestDto user)
        {
            if (ModelState.IsValid)
            {
                Logger.Log($"HOST: {Request.Host.Value} | HOST = {Request.Host.Host} | PORT = {Request.Host.Port}");
                if (user != null)
                {
                    if (await _userRepository.GetAsync(user.Username) == null)
                    {
                        User newUser = new User(user.Username, user.FirstName, user.LastName, SaltHasher.ComputeHash(user.Password));
                        if (user.Image != null)
                        {
                            IFormFile imageFile = user.Image;
                            ImageFile newImage = ImageHelper.CreateUserImageFile(newUser.Id);
                            if (imageFile != null &&
                                 await _imageFileRepository.InsertAsync(newImage))
                            {
                                if (await ImageHelper.SaveUserImageAsync(imageFile, newImage.Url, newImage.ThumbUrl))
                                {
                                    newUser.ImageFileId = newImage.Id;
                                    newUser.ImageFile = newImage;
                                }
                            }
                        }

                        if (await _userRepository.InsertAsync(newUser))
                        {
                            return Ok(new UserDto(newUser,RootPath));
                        }
                        else
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError);
                        }
                    }
                    else
                    {
                        return Ok(MessageExtension.ShowCustomMessage("Sing Up Error!", "User already exists", statusCode: HttpStatusCode.BadRequest));
                    }
                }

                return BadRequest();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
