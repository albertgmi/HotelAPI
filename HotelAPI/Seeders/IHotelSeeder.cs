using HotelAPI.Entities;

namespace HotelAPI.Seeders
{
    public interface IHotelSeeder
    {
        void Seed(HotelDbContext _dbContext);
    }
}
