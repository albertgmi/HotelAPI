namespace HotelAPI.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public string Url { get; set; }
        // Relations
        public int? HotelId { get; set; }
        public Hotel Hotel { get; set; }
        public int? RoomId { get; set; }
        public Room Room { get; set; }
    }
}
