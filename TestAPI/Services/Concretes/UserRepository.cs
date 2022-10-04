using System;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Data;
using TestAPI.ModelContexts;
using TestAPI.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace TestAPI.Services.Concretes
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private TestDbContext _dbContext;

        public UserRepository(TestDbContext dbContext)
            :base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetAsync(string username, string password)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(user => user.Username == username && user.Password == password);
        }

        public async Task<User> GetAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(user => user.Username == username);
        }
    }
}
