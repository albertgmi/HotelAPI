using AutoMapper;
using HotelAPI.Authorizations.HotelAuthorizations;
using HotelAPI.Authorizations;
using HotelAPI.Entities;
using HotelAPI.Exceptions;
using HotelAPI.Models.HotelModels;
using HotelAPI.Models.UserModels;
using HotelAPI.Models;
using HotelAPI.Services.FileServiceFolder;
using Microsoft.AspNetCore.Authorization;
using QuestPDF.Infrastructure;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using HotelAPI.Services.UserServiceFolder;
using HotelAPI.Services.ReportServiceFolder;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Fluent;

namespace HotelAPI.Services.HotelServiceFolder
{
    public class HotelService : IHotelService
    {
        private readonly HotelDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;
        private readonly IReportService _reportService;
        private readonly IFileService _fileService;

        public HotelService(HotelDbContext dbContext, IMapper mapper, IUserContextService userContextService,
            IAuthorizationService authorizationService, IReportService reportService, IFileService fileService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userContextService = userContextService;
            _authorizationService = authorizationService;
            _reportService = reportService;
            _fileService = fileService;
        }

        public PagedResult<HotelDto> GetAll(HotelQuery query)
        {
            var baseHotels = _dbContext
                .Hotels
                .Include(h => h.Address)
                .Include(h => h.Rooms)
                .ThenInclude(r => r.Reservations)
                .Where(x => query.SearchPhrase == null
                            || (x.Name.ToLower().Contains(query.SearchPhrase.ToLower())
                            || (x.Description.ToLower().Contains(query.SearchPhrase.ToLower()))));

            if (!query.SortBy.IsNullOrEmpty())
            {
                var columnSelector = new Dictionary<string, Expression<Func<Hotel, object>>>
                {
                    {nameof(Hotel.Name), x=>x.Name},
                    {nameof(Hotel.Description), x=>x.Description},
                    {nameof(Hotel.Rating), x=>x.Rating},
                    {nameof(Hotel.NumberOfRatings), x=>x.NumberOfRatings}
                };
                var selectedColumn = columnSelector[query.SortBy];

                baseHotels = query.SortDirection == SortDirection.ASC
                    ? baseHotels.OrderBy(selectedColumn)
                    : baseHotels.OrderByDescending(selectedColumn);
            }

            var hotels = baseHotels
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            if (hotels is null)
                throw new NotFoundException("Hotel not found");

            var baseCount = baseHotels.Count();
            var hotelDto = _mapper.Map<List<HotelDto>>(hotels);
            var result = new PagedResult<HotelDto>(hotelDto, baseCount, query.PageSize, query.PageNumber);

            return result;
        }
        public HotelDto GetById(int hotelId)
        {
            var hotel = GetHotelById(hotelId);

            var hotelDto = _mapper.Map<HotelDto>(hotel);
            return hotelDto;
        }
        public int Create(CreateHotelDto dto)
        {
            var hotel = _mapper.Map<Hotel>(dto);
            var hotelId = hotel.Id;

            hotel.CreatedById = (int)_userContextService.GetUserId;

            _dbContext.Hotels.Add(hotel);
            _dbContext.SaveChanges();

            return hotelId;
        }
        public void Update(int hotelId, UpdateHotelDto dto)
        {
            var hotel = GetOnlyHotelById(hotelId);

            var user = _userContextService.User;

            AuthorizedTo(hotel, user, ResourceOperation.Update);

            hotel.Name = dto.Name;
            hotel.Description = dto.Description;
            _dbContext.SaveChanges();
        }
        public void Delete(int hotelId)
        {
            var hotel = GetOnlyHotelById(hotelId);

            var user = _userContextService.User;

            AuthorizedTo(hotel, user, ResourceOperation.Delete);

            _dbContext.Hotels.Remove(hotel);
            _dbContext.SaveChanges();
        }
        public UserDto GetOwner(int hotelId)
        {
            var hotel = GetOnlyHotelById(hotelId);

            var managerId = hotel.CreatedById;
            var manager = _dbContext
                .Users
                .FirstOrDefault(x => x.Id == managerId);
            if (manager is null)
                throw new NotFoundException("Hotel manager not found");
            var managerDto = _mapper.Map<UserDto>(manager);
            return managerDto;
        }
        public void AddRating(int hotelId, decimal rating)
        {
            if (rating < 1 || rating > 5)
                throw new NotInRangeException("Rating has to be in range 1-5");

            var hotel = GetOnlyHotelById(hotelId);

            if (hotel.NumberOfRatings == 0)
            {
                hotel.Rating = rating;
                hotel.NumberOfRatings = 1;
            }
            else
            {
                hotel.NumberOfRatings++;
                hotel.Rating = (hotel.Rating * (hotel.NumberOfRatings - 1) + rating) / hotel.NumberOfRatings;
            }
            _dbContext.SaveChanges();
        }
        public byte[] GenerateReport(int hotelId, DateTime startDate, DateTime endDate)
        {
            var hotel = GetHotelById(hotelId);
            var user = _userContextService.User;
            AuthorizedTo(hotel, user, ResourceOperation.GetReport);

            QuestPDF.Settings.License = LicenseType.Community;
            var report = _reportService.GenerateFullReport(hotel, startDate, endDate);
            var pdf = report.GeneratePdf();
            return pdf;
        }
        public string UploadHotelImage(int hotelId, IFormFile file)
        {
            var hotel = _dbContext
                .Hotels
                .FirstOrDefault(x => x.Id == hotelId);
            if (hotel is null)
                throw new NotFoundException("Hotel not found");

            var url = _fileService.UploadImage(hotelId, null, file);
            _fileService.AddImageToHotel(hotelId, url);
            return url;
        }
        public void DeleteHotelImage(int imageId)
        {
            var image = _dbContext
                .Images
                .FirstOrDefault(x => x.Id == imageId);
            if (image is null)
                throw new NotFoundException("Image not found");
            var url = image.Url;
            _fileService.DeleteImage(url);
            _fileService.DeleteImageFromDb(imageId);
        }
        private void AuthorizedTo(Hotel hotel, ClaimsPrincipal user, ResourceOperation operation)
        {
            var authorizationResult = _authorizationService.AuthorizeAsync(user, hotel,
                new CreatedHotelRequirement(operation)).Result;
            if (!authorizationResult.Succeeded)
                throw new ForbidException("Authorization failed");
        }
        private Hotel GetHotelById(int hotelId)
        {
            var hotel = _dbContext
                .Hotels
                .Include(x => x.Address)
                .Include(x => x.Rooms)
                .ThenInclude(x => x.Reservations)
                .FirstOrDefault(x => x.Id == hotelId);
            if (hotel is null)
                throw new NotFoundException("Hotel not found");
            return hotel;
        }
        private Hotel GetOnlyHotelById(int hotelId)
        {
            var hotel = _dbContext
                .Hotels
                .FirstOrDefault(x => x.Id == hotelId);
            if (hotel is null)
                throw new NotFoundException("Hotel not found");
            return hotel;
        }
    }
}
