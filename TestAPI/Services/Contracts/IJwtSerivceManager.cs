using System;
using System.Threading.Tasks;

namespace TestAPI.Services.Contracts
{
    public interface IJwtSerivceManager
    {
        Task<string> CreateToken(string username,string password);
        Task<bool> SaveIndentity(string email, string username, string password, bool isAdmin = false);
    }
}
