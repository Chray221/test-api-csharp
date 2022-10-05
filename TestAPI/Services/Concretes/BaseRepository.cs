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

        virtual public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new NullReferenceException($"Parammeter \"{typeof(T).Name}Id\" is empty in DeleteAsync");
            }

            T deletedUser = await _dbContext.FindAsync<T>(id);
            if (deletedUser != null)
            {
                deletedUser.IsDeleted = true;
                deletedUser.UpdatedAt = DateTime.Now;
                _dbContext.Update(deletedUser);
            }
            return deletedUser != null;
            
        }

        virtual public async Task<T> GetAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new NullReferenceException($"Parammeter \"{typeof(T).Name}Id\" is empty in GetAsync");
            }
            return await _dbContext.FindAsync<T>(id);
        }

        virtual public async Task<bool> InsertAsync(T data)
        {
            if (data == null)
            {
                throw new NullReferenceException($"Parammeter \"{typeof(T).Name}data\" is null in InsertAsync");
            }

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
            return false;
        }

        virtual public async Task<bool> UpdateAsync(T data)
        {
            if (data == null)
            {
                throw new NullReferenceException($"Parammeter \"{typeof(T).Name}data is null in UpdateAsync");
            }

            if (data != null)
            {
                data.IsDeleted = true;
                data.UpdatedAt = DateTime.Now;
                _dbContext.Update(data);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;

        }
    }
}
