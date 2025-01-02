using AutoMapper;
using HotelAPI.Authorizations.HotelAuthorizations;
using HotelAPI.Authorizations;
using HotelAPI.Entities;
using HotelAPI.Exceptions;
using HotelAPI.Models.RoomModels;
using HotelAPI.Models;
using HotelAPI.Services.FileServiceFolder;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HotelAPI.Services.RoomServiceFolder
{
    public class RoomService : IRoomService
    {
        private readonly HotelDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;
        private readonly IFileService _fileService;
        public RoomService(HotelDbContext dbContext, IMapper mapper, IAuthorizationService authorizationService, IUserContextService userContextService, IFileService fileService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
            _fileService = fileService;
        }

        public PagedResult<RoomDto> GetAll(int hotelId, RoomQuery query)
        {
            var hotel = GetHotelWithRooms(hotelId);
            var baseRooms = hotel
                .Rooms
                .Where(x => query.SearchPhrase == null
                            || (x.Name.ToLower().Contains(query.SearchPhrase.ToLower())
                            || (x.Description.ToLower().Contains(query.SearchPhrase.ToLower())))).AsQueryable();

            if (!query.SortBy.IsNullOrEmpty())
            {
                var columnSelector = new Dictionary<string, Expression<Func<Room, object>>>
                {
                    {nameof(Room.Name), x=>x.Name},
                    {nameof(Room.Description), x=>x.Description},
                    {nameof(Room.Capacity), x=>x.Capacity},
                    {nameof(Room.PricePerNight), x=>x.PricePerNight},
                    {nameof(Room.IsAvailable), x=>x.IsAvailable}
                };
                var selectedColumn = columnSelector[query.SortBy];

                baseRooms = query.SortDirection == SortDirection.ASC
                    ? baseRooms.OrderBy(selectedColumn)
                    : baseRooms.OrderByDescending(selectedColumn);
            }

            var rooms = baseRooms
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();
            if (rooms is null)
                throw new NotFoundException($"Rooms not found in hotel with id {hotelId}");

            var baseCount = baseRooms.Count();
            var roomsDto = _mapper.Map<List<RoomDto>>(rooms);
            var pagedResult = new PagedResult<RoomDto>(roomsDto, baseCount, query.PageSize, query.PageNumber);
            return pagedResult;
        }
        public RoomDto GetById(int hotelId, int roomId)
        {
            var hotel = GetHotelWithRooms(hotelId);
            var room = GetRoomFromHotel(hotel, roomId);
            var roomDto = _mapper.Map<RoomDto>(room);

            return roomDto;
        }
        public int CreateRoom(int hotelId, CreateRoomDto dto)
        {
            var hotel = GetHotelWithRooms(hotelId);
            var user = _userContextService.User;
            AuthorizedTo(hotel, user, ResourceOperation.Create);

            var room = _mapper.Map<Room>(dto);
            room.HotelId = hotel.Id;
            _dbContext.Rooms.Add(room);
            _dbContext.SaveChanges();
            return room.Id;
        }
        public void UpdateRoom(int hotelId, int roomId, UpdateRoomDto dto)
        {
            var hotel = GetHotelWithRooms(hotelId);
            var user = _userContextService.User;

            AuthorizedTo(hotel, user, ResourceOperation.Update);

            var room = GetRoomFromHotel(hotel, roomId);

            room.Name = dto.Name;
            room.Type = dto.Type;
            room.PricePerNight = dto.PricePerNight;
            _dbContext.SaveChanges();
        }
        public void DeleteRoomById(int hotelId, int roomId)
        {
            var hotel = GetHotelWithRooms(hotelId);
            var user = _userContextService.User;
            AuthorizedTo(hotel, user, ResourceOperation.Delete);

            var room = GetRoomFromHotel(hotel, roomId);

            _dbContext.Rooms.Remove(room);
            _dbContext.SaveChanges();
        }
        public void DeleteAllRooms(int hotelId)
        {
            var hotel = GetHotelWithRooms(hotelId);
            var user = _userContextService.User;
            AuthorizedTo(hotel, user, ResourceOperation.Delete);

            var rooms = hotel
                .Rooms
                .ToList();

            _dbContext.Rooms.RemoveRange(rooms);
            _dbContext.SaveChanges();
        }
        public List<RoomDto> GetAvailableRooms(int hotelId, DateTime from, DateTime? to)
        {
            var hotel = GetHotelWithRooms(hotelId);

            if (hotel == null || hotel.Rooms == null || !hotel.Rooms.Any())
                throw new NotFoundException("This hotel has no rooms or does not exist");

            var endDate = to ?? from.AddDays(1);
            if (from >= endDate)
                throw new ArgumentException("Invalid date range: 'from' must be earlier than 'to'.");

            var availableRooms = hotel.Rooms
                .Where(room => room.Reservations
                    .All(reservation => reservation.CheckOutDate <= from || reservation.CheckInDate >= endDate))
                .ToList();
            if (!availableRooms.Any())
                throw new NotFoundException($"Hotel with Id: {hotelId} has no available rooms in the specified date range");

            return _mapper.Map<List<RoomDto>>(availableRooms);
        }
        public string UploadRoomImage(int hotelId, int roomId, IFormFile file)
        {
            var hotel = _dbContext
                .Hotels
                .Include(h => h.Rooms)
                .FirstOrDefault(x => x.Id == hotelId);
            if (hotel is null)
                throw new NotFoundException("Hotel not found");

            var room = GetRoomFromHotel(hotel, roomId);

            var url = _fileService.UploadImage(hotelId, roomId, file);
            _fileService.AddImageToRoom(hotelId, roomId, url);
            return url;
        }
        public void DeleteRoomImage(int imageId)
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
        private Hotel GetHotelWithRooms(int hotelId)
        {
            var hotel = _dbContext
                .Hotels
                .Include(x => x.Address)
                .Include(x => x.Rooms)
                .ThenInclude(r => r.Reservations)
                .FirstOrDefault(x => x.Id == hotelId);
            if (hotel is null)
                throw new NotFoundException("Hotel not found");
            return hotel;
        }
        private Room GetRoomFromHotel(Hotel hotel, int roomId)
        {
            var room = hotel
                .Rooms
                .FirstOrDefault(x => x.Id == roomId);
            var hotelId = hotel.Id;
            if (room is null)
                throw new NotFoundException($"Room with id {roomId} was not found in hotel with id {hotelId}");
            return room;
        }
        private void AuthorizedTo(Hotel hotel, ClaimsPrincipal user, ResourceOperation operation)
        {
            var authorizationResult = _authorizationService.AuthorizeAsync(user, hotel,
                new CreatedHotelRequirement(operation)).Result;
            if (!authorizationResult.Succeeded)
                throw new ForbidException("Authorization failed");
        }
    }
}
