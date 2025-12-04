using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Escort.Chat.API.Services
{
    public class BookingValidationService : IBookingValidationService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BookingValidationService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> IsUserPartOfBooking(string userId, string bookingId)
        {
            var client = _httpClientFactory.CreateClient("BookingAPI");
            try
            {
                var response = await client.GetAsync($"/api/bookings/{bookingId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var booking = JsonSerializer.Deserialize<BookingDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return booking != null && (booking.UserId == userId || booking.EscortId == userId);
                }
            }
            catch (HttpRequestException)
            {
                // Log error
            }
            return false;
        }

        private class BookingDto
        {
            public string UserId { get; set; }
            public string EscortId { get; set; }
        }
    }
}

