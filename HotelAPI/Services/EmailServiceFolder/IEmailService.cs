using HotelAPI.Entities;

namespace HotelAPI.Services.EmailServiceFolder
{
    public interface IEmailService
    {
        Task SendEmailAsync(Hotel hotel, Room room, User user, Reservation reservation);
    }
}
