using System;
using System.Threading.Tasks;
using TestAPI.Data;

namespace TestAPI.Services.Contracts
{
    public interface IBaseRepository<T> where T : BaseModel
    {
        Task<bool> InsertAsync(T data);
        Task<bool> UpdateAsync(T data);
        Task<T> GetAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
    }
}
