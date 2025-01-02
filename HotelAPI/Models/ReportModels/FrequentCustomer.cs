namespace HotelAPI.Models.ReportModels
{
    public class FrequentCustomer
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int ReservationCount { get; set; }
        public override string ToString()
        {
            return $"     - {FullName}, {Email}, {ReservationCount}";
        }
    }
}
