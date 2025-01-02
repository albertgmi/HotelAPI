using HotelAPI.Models.RoomModels;
using HotelAPI.Models;

namespace HotelAPI.Services.RoomServiceFolder
{
    public interface IRoomService
    {
        PagedResult<RoomDto> GetAll(int hotelId, RoomQuery query);
        RoomDto GetById(int hotelId, int roomId);
        int CreateRoom(int hotelId, CreateRoomDto dto);
        void UpdateRoom(int hotelId, int roomId, UpdateRoomDto dto);
        void DeleteRoomById(int hotelId, int roomId);
        void DeleteAllRooms(int hotelId);
        List<RoomDto> GetAvailableRooms(int hotelId, DateTime from, DateTime? to);
        string UploadRoomImage(int hotelId, int roomId, IFormFile file);
        void DeleteRoomImage(int imageId);
    }
}
