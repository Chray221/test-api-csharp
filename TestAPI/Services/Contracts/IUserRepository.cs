using System;
using System.Threading.Tasks;
using TestAPI.Data;

namespace TestAPI.Services.Contracts
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetAsync(string username, string password);
        Task<User> GetAsync(string username);
    }
}
