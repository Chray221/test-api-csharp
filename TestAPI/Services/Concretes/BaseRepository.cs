using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestAPI.Data;
using TestAPI.ModelContexts;
using TestAPI.Services.Contracts;

namespace TestAPI.Services.Concretes
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseModel
    {
        private TestDbContext _dbContext;
        
        public BaseRepository(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            T deletedUser = await _dbContext.FindAsync<T>(id);
            if (deletedUser != null)
            {
                deletedUser.IsDeleted = true;
                deletedUser.UpdatedAt = DateTime.Now;
                _dbContext.Update(deletedUser);
            }
            return deletedUser != null;
        }

        public async Task<T> GetAsync(Guid id)
        {
            return await _dbContext.FindAsync<T>(id);
        }

        public async Task<bool> InsertAsync(T data)
        {
            try
            {
                if (data != null)
                {
                    if(data.Id == Guid.Empty)
                    {
                        data.Id = Guid.NewGuid();
                    }
                    data.CreatedAt = DateTime.Now;
                    await _dbContext.AddAsync(data);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch(Exception ex)
            {
            }
            return false;
        }

        public async Task<bool> UpdateAsync(T data)
        {
            try
            {
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.UpdatedAt = DateTime.Now;
                }
                _dbContext.Update(data);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
