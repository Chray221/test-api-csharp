using System;
using TestAPI.Data;
using TestAPI.ModelContexts;
using TestAPI.Services.Contracts;

namespace TestAPI.Services.Concretes
{
    public class ImageFileRepository : BaseRepository<ImageFile>, IImageFileRepository
    {
        public ImageFileRepository(TestDbContext testDbContext):
            base(testDbContext)
        {
        }
    }
}
