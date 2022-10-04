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
    [ApiVersion("1.1")]
    [Route("v{version:apiversion}/api/Authentication")]
    public class AuthenticationControllerV2 : MyControllerBase
    {
        private IUserRepository _userRepository;
        private IImageFileRepository _imageFileRepository;

        public AuthenticationControllerV2(
            TestDbContext context,
            IWebHostEnvironment environment,
            IUserRepository userRepository,
            IImageFileRepository imageFileRepository)
            :base(context, environment)
        {
            _userRepository = userRepository;
            _imageFileRepository = imageFileRepository;
        }

        // POST api/authentication/sign_in
        [HttpPost("sign_in")]
        [ApiVersion("1.1")]
        public async Task<object> SignInV2([FromBody] SignInUserDto user)
        {
            try
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
                    if (userFound != null)
                    {
                        if (SaltHasher.VerifyHash(user.Password, userFound.Password))
                        {
                            testContext.Entry(userFound).Reference(u => u.ImageFile).Load(); // retreived data from reference entity (ImageFile Property from ImageID)
                            return Ok( new { user = new UserDto(userFound), status = HttpStatusCode.OK });
                        }
                        return Ok(MessageExtension.ShowCustomMessage("Sign In Error", "Username or password mismatched", "Sign Up"));
                    }
                    else
                    {
                        return Ok(MessageExtension.ShowCustomMessage("Sign In Error", "User is not registered", "Sign Up"));
                    }
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        // POST api/authentication/sign_in
        [HttpPut("sign_up")]
        [ApiVersion("1.1")]
        public async Task<object> SignUpV2([FromBody]SignUpUserDto user)
        {
            Logger.Log($"HOST: {Request.Host.Value} | HOST = {Request.Host.Host} | PORT = {Request.Host.Port}");
            if(user != null)
            {
                object required = user.VerifyRequired();
                if (required != null)
                {
                    return BadRequest(required);
                }

                Logger.Log($"QUERY");
                if (await _userRepository.GetAsync(user.Username) == null)
                {
                    User newUser = new User(user.Username, user.FirstName, user.LastName, SaltHasher.ComputeHash(user.Password));
                    if (Request.HasFormContentType &&
                        Request.Form != null &&
                        Request.Form.Files["image"] is IFormFile imageFormFile)
                    {
                        IFormFile imageFile = imageFormFile;
                        string imageName = imageFile.FileName;
                        ImageFile newImage = new ImageFile($"{Guid.NewGuid()}{Path.GetExtension(imageName)}");
                        if (await _imageFileRepository.InsertAsync(newImage) &&
                            imageFile != null)
                        {
                            ImageHelper.SaveThumbImage(imageFile, newImage.ThumbUrl);
                            await ImageHelper.SaveImage(imageFile, newImage.Url);
                            await _imageFileRepository.InsertAsync(newImage);
                            newUser.ImageFileId = newImage.Id;
                            newUser.ImageFile = newImage;
                        }
                    }

                    if (await _userRepository.InsertAsync(newUser))
                    {
                        return Ok(new UserDto(newUser));
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

    }
}
