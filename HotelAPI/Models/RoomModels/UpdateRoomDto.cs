namespace HotelAPI.Models.RoomModels
{
    public class UpdateRoomDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public decimal PricePerNight { get; set; }
    }
}
