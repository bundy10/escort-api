using System.Threading.Tasks;

namespace Escort.Chat.API.Services
{
    public interface IBookingValidationService
    {
        Task<bool> IsUserPartOfBooking(string userId, string bookingId);
    }
}

