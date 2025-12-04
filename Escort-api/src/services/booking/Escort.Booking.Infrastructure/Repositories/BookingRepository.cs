using Escort.Booking.Application.Repositories;
using Escort.Booking.Infrastructure.DBcontext;
using Microsoft.EntityFrameworkCore;

namespace Escort.Booking.Infrastructure.Repositories;

public class BookingRepository : BaseRepository<Domain.Models.Booking>, IBookingRepository
{
    public BookingRepository(BookingDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Domain.Models.Booking>> GetBookingsByClientIdAsync(int clientId)
    {
        return await _dbSet.Where(b => b.ClientId == clientId).ToListAsync();
    }

    public async Task<IEnumerable<Domain.Models.Booking>> GetBookingsByDriverIdAsync(int driverId)
    {
        return await _dbSet.Where(b => b.DriverId == driverId).ToListAsync();
    }
}

