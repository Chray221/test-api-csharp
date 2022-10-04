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
    [Route("v{version:apiversion}/api/Authentication")]
    public class AuthenticationControllerV2 : MyControllerBase
    {
        private IUserRepository _userRepository;
        private IImageFileRepository _imageFileRepository;
        private TestDbContext _dbContext;
        public AuthenticationControllerV2(
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
        public async Task<ActionResult> SignInV2([FromBody] SignInUserRequestDto user)
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
                                return Ok(new { user = new UserDto(userFound), status = HttpStatusCode.OK });
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

        // POST api/authentication/sign_in
        [HttpPut("sign_up")]
        public async Task<ActionResult> SignUpV2([FromBody]SignUpUserRequestDto user)
        {
            if (ModelState.IsValid)
            {
                Logger.Log($"HOST: {Request.Host.Value} | HOST = {Request.Host.Host} | PORT = {Request.Host.Port}");
                if (user != null)
                {
                    if (await _userRepository.GetAsync(user.Username) == null)
                    {
                        User newUser = new User(user.Username, user.FirstName, user.LastName, SaltHasher.ComputeHash(user.Password));
                        if (Request.HasFormContentType &&
                            Request?.Form?.Files["image"] is IFormFile imageFormFile)
                        {
                            IFormFile imageFile = imageFormFile;
                            //string imageName = imageFile.FileName;
                            //ImageFile newImage = new ImageFile($"{Guid.NewGuid()}{Path.GetExtension(imageName)}");
                            ImageFile newImage = new ImageFile(newUser.Id);
                            if (imageFile != null &&
                                 await _imageFileRepository.InsertAsync(newImage))
                            {
                                ImageHelper.SaveThumbImage(imageFile, newImage.ThumbUrl);
                                await ImageHelper.SaveImageAsync(imageFile, newImage.Url);
                                if (await _imageFileRepository.InsertAsync(newImage))
                                {
                                    newUser.ImageFileId = newImage.Id;
                                    newUser.ImageFile = newImage;
                                }
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
            else
            {
                return BadRequest(ModelState);
            }
        }

    }
}
