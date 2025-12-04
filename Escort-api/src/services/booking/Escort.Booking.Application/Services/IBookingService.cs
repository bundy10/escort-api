using BookingEntity = Escort.Booking.Domain.Models.Booking;

namespace Escort.Booking.Application.Services;

public interface IBookingService
{
    Task<IEnumerable<BookingEntity>> GetAllBookingsAsync();
    Task<BookingEntity?> GetBookingByIdAsync(int id);
    Task<BookingEntity> CreateBookingAsync(BookingEntity booking);
    Task<BookingEntity> UpdateBookingAsync(BookingEntity booking);
    Task DeleteBookingAsync(int id);
    Task<BookingEntity> ConfirmBookingAsync(int id);
    Task<BookingEntity> CompleteBookingAsync(int id);
    Task<BookingEntity> CancelBookingAsync(int id);
}
