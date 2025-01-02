using HotelAPI.Models.RoomModels;
using HotelAPI.Services.RoomServiceFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelAPI.Controllers
{
    [Route("/api/hotel/{hotelId}/room")]
    [ApiController]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetAll([FromRoute] int hotelId, [FromQuery] RoomQuery query)
        {
            var rooms = _roomService.GetAll(hotelId, query);
            return Ok(rooms);
        }
        [HttpGet("{roomId}")]
        [AllowAnonymous]
        public ActionResult GetById([FromRoute] int hotelId, [FromRoute] int roomId)
        {
            var room = _roomService.GetById(hotelId, roomId);
            return Ok(room);
        }
        [HttpPost]
        public ActionResult Create([FromRoute] int hotelId, [FromBody] CreateRoomDto roomDto)
        {
            var roomId = _roomService.CreateRoom(hotelId, roomDto);
            return Created($"New room with id {roomId} was created in hotel with id: {hotelId}", null);
        }
        [HttpPut("{roomId}")]
        public ActionResult Update([FromRoute] int hotelId, [FromRoute] int roomId, [FromBody] UpdateRoomDto roomDto)
        {
            _roomService.UpdateRoom(hotelId, roomId, roomDto);
            return Ok();
        }
        [HttpDelete("{roomId}")]
        public ActionResult DeleteById([FromRoute] int hotelId, [FromRoute] int roomId)
        {
            _roomService.DeleteRoomById(hotelId, roomId);
            return NoContent();
        }
        [HttpDelete]
        public ActionResult DeleteAll([FromRoute] int hotelId)
        {
            _roomService.DeleteAllRooms(hotelId);
            return NoContent();
        }
        [HttpGet("available-rooms")]
        public ActionResult GetAvailableRooms([FromRoute] int hotelId, [FromQuery] DateTime from, [FromQuery] DateTime? to)
        {
            var availableRooms = _roomService.GetAvailableRooms(hotelId, from, to);
            return Ok(availableRooms);
        }
        [HttpPost("{roomId}/photo")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult UploadImage([FromRoute] int hotelId, [FromRoute] int roomId, IFormFile file)
        {
            var url = _roomService.UploadRoomImage(hotelId, roomId, file);
            return Created($"New photo with url: {url} has been added", null);
        }
        [HttpDelete("{roomId}/photo/{imageId}")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult DeleteImage([FromRoute] int hotelId, [FromRoute] int roomId, [FromRoute] int imageId)
        {
            _roomService.DeleteRoomImage(imageId);
            return NoContent();
        }
    }
}
