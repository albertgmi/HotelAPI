namespace HotelAPI.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; }

        // Relations
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        public List<Image> Images { get; set; } = new List<Image>();
    }
}
