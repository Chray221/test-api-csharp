using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TestAPI.Models;
using TestAPI.Services.Contracts;

namespace TestAPI.Services.Concretes
{
    internal class JwtSerivceManager : IJwtSerivceManager
    {
        IConfiguration _configuration;
        UserManager<ApplicationUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        SignInManager<ApplicationUser> _signInManager;

        public JwtSerivceManager(IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public bool IsEnabled => _configuration.GetValue<bool>("JWT:IsEnabled");
        public string CreateToken(UserDto user)
        {
            if(user != null)
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                JwtSecurityToken token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            return null;
        }
        public async Task<string> CreateToken(SignInUserRequestDto signInUser)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(signInUser.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, signInUser.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                //if user has many roles
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                JwtSecurityToken token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                //SignInResult result = await _signInManager.PasswordSignInAsync(user, signInUser.Password, false, false);
                //if (!result.Succeeded)
                //{
                //    throw new Exception(result.ToString());
                //}

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            return null;
        }

        public async Task<bool> SaveIndentity(SignUpUserRequestDto signUpUser, bool isAdmin = false)
        {
            ApplicationUser userExists = await _userManager.FindByNameAsync(signUpUser.Username);
            if (userExists != null)
            {
                return false;
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = signUpUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = signUpUser.Username,
            };
            IdentityResult result = await _userManager.CreateAsync(user, signUpUser.Password);
            if(!result.Succeeded)
            {                
                throw new Exception(result.ToString());
            }

            if (isAdmin)
            {
                if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin);
                }
            }
            //else
            //{
            //    if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            //        await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            //    await _userManager.AddToRoleAsync(user, UserRoles.User);
            //}
            return result.Succeeded;
        }

        #region V2
        public async Task<ApplicationUser> Authenticate(SignInUserRequestDto signInUser)
        {
            if (signInUser != null)
            {

                ApplicationUser user = await _userManager.FindByNameAsync(signInUser.Username);
                if (user != null && await _userManager.CheckPasswordAsync(user, signInUser.Password))
                {
                    return user;
                }
            }
            return null;
        }

        public async Task<string> GenerateToken(ApplicationUser user)
        {
            SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            SigningCredentials signingCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);
                List<Claim> authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                };
            
            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
    }
}
