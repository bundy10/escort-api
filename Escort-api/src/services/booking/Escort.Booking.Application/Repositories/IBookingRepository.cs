using BookingEntity = Escort.Booking.Domain.Models.Booking;

namespace Escort.Booking.Application.Repositories;

public interface IBookingRepository : IRepository<BookingEntity>
{
    Task<IEnumerable<BookingEntity>> GetBookingsByClientIdAsync(int clientId);
    Task<IEnumerable<BookingEntity>> GetBookingsByDriverIdAsync(int driverId);
}
