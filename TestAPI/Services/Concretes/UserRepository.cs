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

        public override async Task<User> GetAsync(Guid id)
        {
            User userFound = await base.GetAsync(id);
            GetUserImage(userFound);
            return userFound;
        }

        public async Task<User> GetAsync(string username, string password)
        {
            User userFound = await _dbContext.User.FirstOrDefaultAsync(user => user.Username == username && user.Password == password);
            GetUserImage(userFound);
            return userFound;
        }

        public async Task<User> GetAsync(string username)
        {
            User userFound = await _dbContext.User.FirstOrDefaultAsync(user => user.Username == username);
            GetUserImage(userFound);
            return userFound;
        }

        public async Task<bool> IsUserNameExistAsync(string username)
        {
            User userFound = await _dbContext.User.FirstOrDefaultAsync(user => user.Username == username);
            return userFound != null;
        }

        private void GetUserImage(User userFound)
        {
            if (userFound != null)
            {
                // retreived data from reference entity (ImageFile Property from ImageFileId)
                _dbContext.Entry(userFound).Reference(u => u.ImageFile).Load();
            }
        }
    }
}
