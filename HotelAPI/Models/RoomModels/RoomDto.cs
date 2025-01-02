using HotelAPI.Models.ReservationModels;

namespace HotelAPI.Models.RoomModels
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; }
        public List<ReservationDto> Reservations { get; set; }
    }
}
