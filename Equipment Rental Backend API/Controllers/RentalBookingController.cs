using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using Service.Interface;
using Equipment_Rental_Backend_API.Dtos.RentalBooking;
using Equipment_Rental_Backend_API.Dtos.Response;
using Domain.Models;
namespace Equipment_Rental_Backend_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalBookingController : ControllerBase
    {
        private readonly IRentalBookingService _rentalBookingService;

        public RentalBookingController(IRentalBookingService rentalBookingService)
        {
            _rentalBookingService = rentalBookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRentalBookings([FromQuery] string? currency = null)
        {
            var rentalBookings = await _rentalBookingService.GetAllRentalBookings(currency);
            if (rentalBookings == null)
            {
                return NotFound(new ErrorResponse
                {
                    Code = "NotFound",
                    Message = $"Failed to retrieve bookings."
                });
            }
            var rentalBookingDtos = rentalBookings.Adapt<IEnumerable<RentalBookingDto>>();
            return Ok(rentalBookingDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRentalBookingById(int id, [FromQuery] string? currency = null)
        {
            var rentalBooking = await _rentalBookingService.GetRentalBookingById(id, currency);
            if (rentalBooking == null)
            {
                return NotFound(new ErrorResponse
                {
                    Code = "NotFound",
                    Message = $"Booking with ID {id} not found."
                });
            }
            var rentalBookingDto = rentalBooking.Adapt<RentalBookingDto>();
            return Ok(rentalBookingDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRentalBooking(CreateRentalBookingDto createRentalBookingDto, [FromQuery] string? currency = null)
        {
            var createdRentalBooking = await _rentalBookingService.CreateRentalBooking(createRentalBookingDto.Adapt<RentalBooking>(), currency);

            if (createdRentalBooking == null)
                return BadRequest(new ErrorResponse
                {
                    Code = "CreationFailed",
                    Message = "Failed to create rental booking."
                });

            return CreatedAtAction(nameof(GetRentalBookings), new { /* id = createdRentalBooking.Id */ }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRentalBooking(int id, UpdateRentalBookingDto rentalBookingDto)
        {
            var updatedRentalBooking = rentalBookingDto.Adapt<RentalBooking>();

            if (updatedRentalBooking == null)
            {
                return BadRequest(new ErrorResponse
                {
                    Code = "UpdateFailed",
                    Message = "Failed to update rental booking."
                });
            }

            return Ok(await _rentalBookingService.UpdateRentalBooking(id, updatedRentalBooking));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRentalBooking(int id)
        {
            await _rentalBookingService.DeleteRentalBooking(id);
            return NoContent();
        }

        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmRentalBooking(int id)
        {
            await _rentalBookingService.ConfirmRentalBooking(id);
            return NoContent();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelRentalBooking(int id)
        {
            await _rentalBookingService.CancelRentalBooking(id);
            return NoContent();
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteRentalBooking(int id)
        {
            await _rentalBookingService.CompleteRentalBooking(id);
            return NoContent();
        }
    }
}
