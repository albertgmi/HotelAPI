using HotelAPI.Entities;
using QuestPDF.Fluent;

namespace HotelAPI.Services.ReportServiceFolder
{
    public interface IReportService
    {
        Document GenerateFullReport(Hotel hotel, DateTime startDate, DateTime endDate);
    }
}
