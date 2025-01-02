namespace HotelAPI.Models.ReportModels
{
    public class CustomerReport
    {
        public int TotalCustomers { get; set; }
        public string FrequentCustomers { get; set; }
        public double AverageReservationsPerCustomer { get; set; }
    }
}
