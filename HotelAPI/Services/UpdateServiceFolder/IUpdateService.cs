using HotelAPI.Entities;

namespace HotelAPI.Services.UpdateServiceFolder
{
    public interface IUpdateService
    {
        void Update(HotelDbContext _dbContext);
    }
}
