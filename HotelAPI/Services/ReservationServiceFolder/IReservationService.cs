using HotelAPI.Models.ReservationModels;

namespace HotelAPI.Services.ReservationServiceFolder
{
    public interface IReservationService
    {
        List<ReservationDto> GetAll(int hotelId, int roomId);
        ReservationDto GetById(int hotelId, int roomId, int reservationId);
        Task<int> CreateAsync(int hotelId, int roomId, CreateReservationDto dto);
        void Delete(int hotelId, int roomId, int reservationId);
        void Update(int hotelId, int roomId, int reservationId, UpdateReservationDto dto);
    }
}
