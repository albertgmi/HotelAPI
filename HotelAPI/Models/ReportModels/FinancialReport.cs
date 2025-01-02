namespace HotelAPI.Models.ReportModels
{
    public class FinancialReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TotalRevenue { get; set; }
        public int ReservationCount { get; set; }
        public string AverageRevenuePerReservation { get; set; }
    }
}
