namespace HotelAPI.Models.HotelModels
{
    public class CreateHotelDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string ContactNumber { get; set; }
    }
}
