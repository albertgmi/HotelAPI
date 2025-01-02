namespace HotelAPI.Models.ReportModels
{
    public class OccupancyReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public string OccupancyRate { get; set; }
    }
}
