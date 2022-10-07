using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Helpers;
using TestAPI.ModelContexts;
using TestAPI.Data;
using TestAPI.Models;
using TestAPI.Services.Contracts;
using Swashbuckle.AspNetCore.Annotations;

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
        /// <summary>
        /// Sign in user using username and password json
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("sign_in")]
        [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, Type = typeof(UserDto))]
        [SwaggerOperation(Summary = "Sign in user using username and password json")]
        public async Task<ActionResult> SignIn(
            [FromBody] SignInUserRequestDto user,
            [FromServices] IJwtSerivceManager jwtSerivceManager)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (user != null)
                    {
                        User userFound = await _userRepository.GetAsync(user.Username);

                        if(userFound == null)
                        {
                            return BadRequest(MessageExtension.ShowCustomMessage("Sign In Error", "User is not registered", "Sign Up", statusCode: HttpStatusCode.BadRequest));
                        }
                        if (jwtSerivceManager.IsEnabled)
                        {
                            string token = await jwtSerivceManager.CreateToken(user);
                            if (string.IsNullOrEmpty(token))
                            {
                                return StatusCode((int)HttpStatusCode.InternalServerError, MessageExtension.ShowCustomMessage("Sign In Error", "Something went wrong", "Okay", statusCode: HttpStatusCode.InternalServerError));
                            }

                            if (SaltHasher.VerifyHash(user.Password, userFound.Password))
                            {
                                UserDto userDto = new UserDto(userFound, this.GetRootUrl());
                                userDto.Token = token;
                                return Ok(userDto);
                            }
                        }

                        return BadRequest(MessageExtension.ShowCustomMessage("Sign In Error", "Username or password mismatched", "Okay", statusCode: HttpStatusCode.BadRequest));
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
        [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, Type = typeof(UserDto))]
        public async Task<ActionResult> SignUp(
            [FromBody]SignUpUserRequestDto user,
            [FromServices] IJwtSerivceManager jwtSerivceManager)
        {
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    if (await _userRepository.IsUserNameExistAsync(user.Username))
                    {
                        return BadRequest(MessageExtension.ShowCustomMessage("Sing Up Error!", "Username already taken.", statusCode: HttpStatusCode.BadRequest));
                    }

                    if (jwtSerivceManager.IsEnabled)
                    {
                        if (!await jwtSerivceManager.SaveIndentity(user))
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError, MessageExtension.ShowCustomMessage("Login Error", "Something went wrong!", statusCode: HttpStatusCode.InternalServerError));
                        }
                    }
                    else
                    {

                    }
                    User newUser = new User(user.Username, user.FirstName, user.LastName, SaltHasher.ComputeHash(user.Password), user.Email);
                    if (await _userRepository.InsertAsync(newUser))
                    {
                        return Ok(new UserDto(newUser, this.GetRootUrl()));
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError);
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
        [SwaggerResponse(statusCode: (int)HttpStatusCode.OK, Type = typeof(UserDto))]
        public async Task<ActionResult> SignUpForm(
            [FromForm] SignUpUserRequestDto user,
            [FromServices] IJwtSerivceManager jwtSerivceManager)
        {
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    if (await _userRepository.IsUserNameExistAsync(user.Username))
                    {
                        return BadRequest(MessageExtension.ShowCustomMessage("Sing Up Error!", "Username already taken.", statusCode: HttpStatusCode.BadRequest));
                    }
                    if (jwtSerivceManager.IsEnabled)
                    {
                        if (!await jwtSerivceManager.SaveIndentity(user))
                        {
                            return StatusCode((int)HttpStatusCode.InternalServerError, MessageExtension.ShowCustomMessage("Login Error", "Something went wrong!", statusCode: HttpStatusCode.InternalServerError));
                        }
                    }
                    else
                    {

                    }

                    User newUser = new User(user.Username, user.FirstName, user.LastName, SaltHasher.ComputeHash(user.Password), user.Email);
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
                        return Ok(new UserDto(newUser, this.GetRootUrl()));
                    }
                    else
                    {
                        return StatusCode((int)HttpStatusCode.InternalServerError);
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
