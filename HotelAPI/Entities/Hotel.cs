namespace HotelAPI.Entities
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Rating { get; set; }
        public int NumberOfRatings { get; set; }
        public string ContactNumber { get; set; }

        // Relations
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<Image> Images { get; set; } = new List<Image>();
    }
}
