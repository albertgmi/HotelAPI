using AutoMapper;
using HotelAPI.Entities;
using HotelAPI.Models.HotelModels;
using HotelAPI.Models.ReservationModels;
using HotelAPI.Models.RoomModels;
using HotelAPI.Models.UserModels;

namespace HotelAPI.Profiles
{
    public class HotelMappingProfile : Profile
    {
        public HotelMappingProfile()
        {
            CreateMap<Hotel, HotelDto>()
                .ForMember(dto => dto.City, h => h.MapFrom(x => x.Address.City))
                .ForMember(dto => dto.Street, h => h.MapFrom(x => x.Address.Street))
                .ForMember(dto => dto.PostalCode, h => h.MapFrom(x => x.Address.PostalCode));
            CreateMap<CreateHotelDto, Hotel>()
                .ForMember(r => r.Address, dto => dto.MapFrom(x => new Address()
                { City = x.City, Street = x.Street, PostalCode = x.PostalCode }));

            CreateMap<Room, RoomDto>();
            CreateMap<CreateRoomDto, Room>();

            CreateMap<User, UserDto>();

            CreateMap<Reservation, ReservationDto>();
            CreateMap<CreateReservationDto, ReservationDto>();
        }
    }
}
