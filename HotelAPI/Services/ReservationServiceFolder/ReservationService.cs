using AutoMapper;
using HotelAPI.Authorizations.ReservationAuthorizations;
using HotelAPI.Authorizations;
using HotelAPI.Entities;
using HotelAPI.Exceptions;
using HotelAPI.Models.ReservationModels;
using HotelAPI.Services.EmailServiceFolder;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using HotelAPI.Services.UserServiceFolder;

namespace HotelAPI.Services.ReservationServiceFolder
{
    public class ReservationService : IReservationService
    {
        private readonly HotelDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;
        private readonly IEmailService _emailService;

        public ReservationService(HotelDbContext dbContext, IMapper mapper, IAuthorizationService authorizationService,
            IUserContextService userContextService, IEmailService emailService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
            _emailService = emailService;
        }
        public List<ReservationDto> GetAll(int hotelId, int roomId)
        {
            var reservations = GetReservationsFromHotelRoom(hotelId, roomId);
            var reservationsDto = _mapper.Map<List<ReservationDto>>(reservations);
            return reservationsDto;

        }
        public ReservationDto GetById(int hotelId, int roomId, int reservationId)
        {
            var reservation = GetReservationsFromHotelRoom(hotelId, roomId)
                .FirstOrDefault(x => x.Id == reservationId);
            var reservationDto = _mapper.Map<ReservationDto>(reservation);
            return reservationDto;
        }
        public async Task<int> CreateAsync(int hotelId, int roomId, CreateReservationDto dto)
        {
            if (dto.CheckInDate >= dto.CheckOutDate)
                throw new BadDateException("Check-out date must be later than check-in date.");

            var hotel = _dbContext
                .Hotels
                .Include(h => h.Rooms)
                .ThenInclude(x => x.Reservations)
                .FirstOrDefault(x => x.Id == hotelId);
            if (hotel is null)
                throw new NotFoundException("Hotel was not found");

            var room = hotel
                .Rooms
                .FirstOrDefault(room => room.Id == roomId);
            if (room is null)
                throw new NotFoundException($"Room with id {roomId} not found in hotel with id {hotelId}.");

            var isRoomAvailable = !await _dbContext.Reservations
                .AnyAsync(reservation => reservation.RoomId == roomId &&
                                         reservation.CheckInDate < dto.CheckOutDate &&
                                         reservation.CheckOutDate > dto.CheckInDate);
            if (!isRoomAvailable)
                throw new RoomNotAvailableException("The room is not available for the selected dates.");

            var days = (dto.CheckOutDate - dto.CheckInDate).Days;
            var totalPrice = room.PricePerNight * days;

            var reservation = new Reservation()
            {
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                RoomId = roomId,
                TotalPrice = totalPrice,
                Status = "Pending"
            };

            reservation.MadeById = (int)_userContextService.GetUserId;

            var user = _userContextService.User;
            AuthorizedTo(reservation, user, ResourceOperation.Create);

            var userId = _userContextService.GetUserId;
            var userFromDb = await _dbContext
                .Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (userFromDb is null)
                throw new NotFoundException("User not found");

            await _emailService.SendEmailAsync(hotel, room, userFromDb, reservation);
            _dbContext.Reservations.Add(reservation);
            await _dbContext.SaveChangesAsync();

            return reservation.Id;
        }

        public void Delete(int hotelId, int roomId, int reservationId)
        {
            var reservation = GetReservationsFromHotelRoom(hotelId, roomId)
               .FirstOrDefault(x => x.Id == reservationId);

            var user = _userContextService.User;
            AuthorizedTo(reservation, user, ResourceOperation.Delete);

            _dbContext.Remove(reservation);
            _dbContext.SaveChanges();
        }
        public void Update(int hotelId, int roomId, int reservationId, UpdateReservationDto dto)
        {
            if (dto.CheckInDate < DateTime.Now)
                throw new BadDateException("You can't make reservation in the past.");
            var reservation = GetReservationsFromHotelRoom(hotelId, roomId)
               .FirstOrDefault(x => x.Id == reservationId);

            var user = _userContextService.User;
            AuthorizedTo(reservation, user, ResourceOperation.Create);

            var isRoomAvailable = !_dbContext.Reservations
            .Any(reservation => reservation.RoomId == roomId &&
                                reservation.CheckInDate < dto.CheckOutDate &&
                                reservation.CheckOutDate > dto.CheckInDate);
            if (!isRoomAvailable)
                throw new RoomNotAvailableException("The room is already reserved for the selected dates.");
            reservation.CheckInDate = dto.CheckInDate;
            reservation.CheckOutDate = dto.CheckOutDate;
            _dbContext.SaveChanges();
        }
        private List<Reservation> GetReservationsFromHotelRoom(int hotelId, int roomId)
        {
            var hotel = _dbContext
                .Hotels
                .Include(x => x.Rooms)
                .ThenInclude(x => x.Reservations)
                .FirstOrDefault(x => x.Id == hotelId);
            if (hotel is null)
                throw new NotFoundException($"Hotel was not found");

            var room = hotel
                .Rooms
                .FirstOrDefault(x => x.Id == roomId);
            if (room is null)
                throw new NotFoundException($"Room with id {roomId} was not found in hotel with id {hotelId}");

            var reservations = room
                .Reservations
                .ToList();
            return reservations;
        }
        private void AuthorizedTo(Reservation reservation, ClaimsPrincipal user, ResourceOperation operation)
        {
            var authorizationResult = _authorizationService.AuthorizeAsync(user, reservation,
                new CreatedReservationRequirement(operation)).Result;
            if (!authorizationResult.Succeeded)
                throw new ForbidException("Authorization failed");
        }
    }
}
