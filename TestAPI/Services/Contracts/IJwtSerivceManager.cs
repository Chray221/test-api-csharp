using System;
using System.Threading.Tasks;
using TestAPI.Models;

namespace TestAPI.Services.Contracts
{
    public interface IJwtSerivceManager
    {
        Task<string> CreateToken(SignInUserRequestDto user);
        Task<bool> SaveIndentity(SignUpUserRequestDto signUpUser, bool isAdmin = false);
        bool IsEnabled { get; }
    }
}
