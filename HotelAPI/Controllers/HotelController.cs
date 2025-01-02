using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace HotelAPI.Controllers
{
    [Route("/api/hotel")]
    [ApiController]
    [Authorize]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetAll([FromQuery] HotelQuery query)
        {
            var result = _hotelService.GetAll(query);
            return Ok(result);
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult GetById([FromRoute] int id)
        {
            var result = _hotelService.GetById(id);
            return Ok(result);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([FromBody] CreateHotelDto hotelDto)
        {
            var hotelId = _hotelService.Create(hotelDto);
            return Created($"Hotel with id: {hotelId} created", null);
        }
        [HttpPut("{hotelId}")]
        public ActionResult Update([FromRoute] int hotelId, [FromBody] UpdateHotelDto hotelDto)
        {
            _hotelService.Update(hotelId, hotelDto);
            return NoContent();
        }
        [HttpDelete("{hotelId}")]
        public ActionResult Delete([FromRoute] int hotelId)
        {
            _hotelService.Delete(hotelId);
            return NoContent();
        }
        [HttpGet("{hotelId}/owner")]
        [AllowAnonymous]
        public ActionResult GetOwner([FromRoute] int hotelId)
        {
            var owner = _hotelService.GetOwner(hotelId);
            return Ok(owner);
        }
        [HttpPost("{hotelId}/add-rating")]
        [Authorize]
        public ActionResult AddRating([FromRoute] int hotelId, [FromQuery] decimal rating)
        {
            _hotelService.AddRating(hotelId, rating);
            return Ok();
        }
        [HttpGet("{hotelId}/generate-report")]
        public ActionResult GetReport([FromRoute] int hotelId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var fullReport = _hotelService.GenerateReport(hotelId, startDate, endDate);
            return File(fullReport, "application/pdf", $"HotelReport_hotelId_{hotelId}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
        }
        [HttpPost("{hotelId}/photo")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult UploadImage([FromRoute] int hotelId, IFormFile file)
        {
            var url = _hotelService.UploadHotelImage(hotelId, file);
            return Created($"New photo with url: {url} has been added", null);
        }
        [HttpDelete("{hotelId}/photo/{imageId}")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult DeleteImage([FromRoute] int hotelId, [FromRoute] int imageId)
        {
            _hotelService.DeleteHotelImage(imageId);
            return NoContent();
        }
    }
}
