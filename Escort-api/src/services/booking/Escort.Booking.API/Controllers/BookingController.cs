using Escort.Booking.API.DTO;
using Escort.Booking.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Escort.Booking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingController> _logger;

    public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingGetDTO>>> GetAllBookings()
    {
        try
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            var bookingDtos = bookings.Select(BookingMapper.ToGetDTO);
            return Ok(bookingDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings");
            return StatusCode(500, "An error occurred while retrieving bookings");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingGetDTO>> GetBookingById(int id)
    {
        try
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found");
            }

            return Ok(BookingMapper.ToGetDTO(booking));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking {BookingId}", id);
            return StatusCode(500, "An error occurred while retrieving the booking");
        }
    }

    [HttpPost]
    public async Task<ActionResult<BookingGetDTO>> CreateBooking([FromBody] BookingPostPutDto bookingDto)
    {
        try
        {
            var booking = BookingMapper.ToModel(bookingDto);
            var createdBooking = await _bookingService.CreateBookingAsync(booking);
            var resultDto = BookingMapper.ToGetDTO(createdBooking);

            return CreatedAtAction(nameof(GetBookingById), new { id = createdBooking.Id }, resultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            return StatusCode(500, "An error occurred while creating the booking");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookingGetDTO>> UpdateBooking(int id, [FromBody] BookingPostPutDto bookingDto)
    {
        try
        {
            var existingBooking = await _bookingService.GetBookingByIdAsync(id);
            if (existingBooking == null)
            {
                return NotFound($"Booking with ID {id} not found");
            }

            var booking = BookingMapper.ToModel(bookingDto);
            booking.Id = id;
            booking.Status = existingBooking.Status; // Preserve existing status
            booking.BookingTime = existingBooking.BookingTime; // Preserve booking time

            var updatedBooking = await _bookingService.UpdateBookingAsync(booking);
            return Ok(BookingMapper.ToGetDTO(updatedBooking));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking {BookingId}", id);
            return StatusCode(500, "An error occurred while updating the booking");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBooking(int id)
    {
        try
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found");
            }

            await _bookingService.DeleteBookingAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking {BookingId}", id);
            return StatusCode(500, "An error occurred while deleting the booking");
        }
    }

    [HttpPut("{id}/confirm")]
    public async Task<ActionResult<BookingGetDTO>> ConfirmBooking(int id)
    {
        try
        {
            var confirmedBooking = await _bookingService.ConfirmBookingAsync(id);
            return Ok(BookingMapper.ToGetDTO(confirmedBooking));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Booking {BookingId} not found", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation for booking {BookingId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming booking {BookingId}", id);
            return StatusCode(500, "An error occurred while confirming the booking");
        }
    }

    [HttpPut("{id}/complete")]
    public async Task<ActionResult<BookingGetDTO>> CompleteBooking(int id)
    {
        try
        {
            var completedBooking = await _bookingService.CompleteBookingAsync(id);
            return Ok(BookingMapper.ToGetDTO(completedBooking));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Booking {BookingId} not found", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation for booking {BookingId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing booking {BookingId}", id);
            return StatusCode(500, "An error occurred while completing the booking");
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<BookingGetDTO>> CancelBooking(int id)
    {
        try
        {
            var cancelledBooking = await _bookingService.CancelBookingAsync(id);
            return Ok(BookingMapper.ToGetDTO(cancelledBooking));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Booking {BookingId} not found", id);
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation for booking {BookingId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
            return StatusCode(500, "An error occurred while cancelling the booking");
        }
    }
}

