namespace HotelAPI.Models.RoomModels
{
    public class CreateRoomDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
