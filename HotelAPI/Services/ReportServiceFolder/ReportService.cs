using HotelAPI.Entities;
using HotelAPI.Exceptions;
using HotelAPI.Models.ReportModels;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Text;

namespace HotelAPI.Services.ReportServiceFolder
{
    public class ReportService : IReportService
    {
        private readonly HotelDbContext _dbContext;

        public ReportService(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Document GenerateFullReport(Hotel hotel, DateTime startDate, DateTime endDate)
        {
            var occupancyReport = GenerateOccupancyReport(hotel, startDate, endDate);
            var financialReport = GenerateFinancialReport(hotel, startDate, endDate);
            var customerReport = GenerateCustomerReport(hotel, startDate, endDate);

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    page.Header().Column(col =>
                    {
                        col.Item().Text($"Hotel Report: {hotel.Name}")
                            .SemiBold().FontSize(20).FontColor(Colors.Red.Medium);

                        col.Item().Text($"Covering the dates: {startDate.ToString("yyyy-MM-dd")} to {endDate.ToString("yyyy-MM-dd")}\n")
                            .FontSize(14).FontColor(Colors.Blue.Darken2);
                    });


                    page.Content().Column(col =>
                    {
                        col.Item().Text("Occupancy Report\n").Bold().FontSize(16);
                        col.Item().Text($"Total Rooms: {occupancyReport.TotalRooms}");
                        col.Item().Text($"Occupied Rooms: {occupancyReport.OccupiedRooms}");
                        col.Item().Text($"Occupancy Rate: {occupancyReport.OccupancyRate} \n");

                        col.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2);

                        col.Item().Text("\nFinancial Report\n").Bold().FontSize(16);
                        col.Item().Text($"Total Revenue: {financialReport.TotalRevenue}");
                        col.Item().Text($"Reservation Count: {financialReport.ReservationCount}");
                        col.Item().Text($"Average Revenue per Reservation: {financialReport.AverageRevenuePerReservation}\n");

                        col.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2);

                        col.Item().Text("\nCustomer Report\n").Bold().FontSize(16);
                        col.Item().Text($"Total number of guests: {customerReport.TotalCustomers}");
                        col.Item().Text($"Frequent guests (more than 3 reservations)");
                        col.Item().Text($"Includes name, email and reservation count:").Bold();
                        col.Item().Text($"{customerReport.FrequentCustomers}");
                        col.Item().Text($"Average reservations per guest: {customerReport.AverageReservationsPerCustomer}\n");
                    });

                    page.Footer().AlignRight().Text(text =>
                    {
                        text.Span("Generated on: ");
                        text.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm")).FontColor(Colors.Grey.Darken2);
                    });
                });
            });
        }
        private OccupancyReport GenerateOccupancyReport(Hotel hotel, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new BadDateException("The start date cannot be later than the end date.");

            var totalRooms = GetTotalNumberOfRooms(hotel);
            var hotelRoomIds = GetHotelRoomIds(hotel);
            var occupiedRooms = _dbContext
                .Reservations
                .Where(reservation => reservation.CheckInDate <= endDate
                                      && reservation.CheckOutDate >= startDate
                                      && hotelRoomIds.Contains(reservation.RoomId))
                .Select(reservation => reservation.RoomId)
                .Distinct()
                .ToList()
                .Count;

            var occupancyRate = totalRooms > 0 ? (double)occupiedRooms / totalRooms * 100 : 0;

            return new OccupancyReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRooms = totalRooms,
                OccupiedRooms = occupiedRooms,
                OccupancyRate = $"{occupancyRate:F2}%"
            };
        }
        private FinancialReport GenerateFinancialReport(Hotel hotel, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new BadDateException("The start date cannot be later than the end date.");

            var hotelRoomIds = GetHotelRoomIds(hotel);

            var reservations = GetReservationsFromHotel(hotelRoomIds, startDate, endDate);

            if (!reservations.Any())
                throw new NotFoundException($"No reservations found for {hotel.Name} between {startDate} and {endDate}");

            decimal totalRevenue = 0;
            foreach (var reservation in reservations)
            {
                var numberOfDays = (decimal)Math.Ceiling((reservation.CheckOutDate - reservation.CheckInDate).TotalDays);
                totalRevenue += reservation.Room.PricePerNight * numberOfDays;
            }

            var averageRevenue = reservations.Count > 0 ? totalRevenue / reservations.Count : 0;

            return new FinancialReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = totalRevenue.ToString("C", new CultureInfo("pl-PL")),
                ReservationCount = reservations.Count,
                AverageRevenuePerReservation = averageRevenue.ToString("C", new CultureInfo("pl-PL"))
            };
        }
        private CustomerReport GenerateCustomerReport(Hotel hotel, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new BadDateException("The start date cannot be later than the end date.");

            var customers = _dbContext.Users
                .Include(u => u.Reservations)
                .ThenInclude(r => r.Room)
                .ThenInclude(room => room.Hotel)
                .Where(u => u.Reservations
                            .Any(r => r.Room.Hotel.Id == hotel.Id &&
                                      r.CheckInDate <= endDate &&
                                      r.CheckOutDate >= startDate))
                .ToList();

            if (!customers.Any())
                throw new NotFoundException($"Customers not found in hotel {hotel.Name}");

            var frequentCustomers = customers
                .Where(x => x.Reservations.Count > 3)
                .Select(x => new FrequentCustomer
                {
                    FullName = $"{x.FirstName} {x.LastName}",
                    Email = x.Email,
                    ReservationCount = x.Reservations.Count
                }).ToList();

            if (!frequentCustomers.Any())
                throw new NotFoundException($"Customers not found in hotel {hotel.Name}");

            var sb = new StringBuilder();
            foreach (var customer in frequentCustomers)
            {
                sb.AppendLine(customer.ToString());
            }

            return new CustomerReport
            {
                TotalCustomers = customers.Count,
                FrequentCustomers = sb.ToString(),
                AverageReservationsPerCustomer = customers.Any() ? customers.Average(c => c.Reservations.Count) : 0
            };
        }
        private int GetTotalNumberOfRooms(Hotel hotel)
        {
            var totalRooms = hotel
                .Rooms
                .ToList()
                .Count;
            return totalRooms;
        }
        private List<int> GetHotelRoomIds(Hotel hotel)
        {
            var hotelRoomIds = hotel
                .Rooms
                .Select(room => room.Id)
                .ToList();
            if (!hotelRoomIds.Any())
                throw new NotFoundException($"No rooms in {hotel.Name}");
            return hotelRoomIds;
        }
        private List<Reservation> GetReservationsFromHotel(List<int> hotelRoomIds, DateTime startDate, DateTime endDate)
        {
            var reservations = _dbContext
                .Reservations
                .Include(r => r.Room)
                .Where(x => x.CheckInDate <= endDate
                         && x.CheckOutDate >= startDate
                         && hotelRoomIds.Contains(x.RoomId))
                .ToList();
            if (!reservations.Any())
                throw new NotFoundException("Reservations not found");
            return reservations;
        }
    }
}
