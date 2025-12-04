using System.Net.Http.Json;
using Escort.Booking.Application.Repositories;
using Escort.Booking.Domain.Enums;
using BookingEntity = Escort.Booking.Domain.Models.Booking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Escort.Booking.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IBookingRepository bookingRepository,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IEnumerable<BookingEntity>> GetAllBookingsAsync()
    {
        return await _bookingRepository.GetAllAsync();
    }

    public async Task<BookingEntity?> GetBookingByIdAsync(int id)
    {
        return await _bookingRepository.GetByIdAsync(id);
    }

    public async Task<BookingEntity> CreateBookingAsync(BookingEntity booking)
    {
        // Set initial status to Pending
        booking.Status = BookingStatus.Pending;
        booking.BookingTime = DateTime.UtcNow;
        
        return await _bookingRepository.AddAsync(booking);
    }

    public async Task<BookingEntity> UpdateBookingAsync(BookingEntity booking)
    {
        return await _bookingRepository.UpdateAsync(booking);
    }

    public async Task DeleteBookingAsync(int id)
    {
        await _bookingRepository.DeleteAsync(id);
    }

    public async Task<BookingEntity> ConfirmBookingAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
        {
            throw new KeyNotFoundException($"Booking with ID {id} not found");
        }

        if (booking.Status != BookingStatus.Pending)
        {
            throw new InvalidOperationException($"Cannot confirm booking with status {booking.Status}");
        }

        // Update status to Confirmed
        booking.Status = BookingStatus.Confirmed;
        await _bookingRepository.UpdateAsync(booking);

        // Trigger payment capture via HTTP call to Payment API
        try
        {
            await CapturePaymentAsync(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to capture payment for booking {BookingId}", id);
            // Rollback status change
            booking.Status = BookingStatus.Pending;
            await _bookingRepository.UpdateAsync(booking);
            throw new InvalidOperationException("Failed to capture payment. Booking remains pending.", ex);
        }

        return booking;
    }

    public async Task<BookingEntity> CompleteBookingAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
        {
            throw new KeyNotFoundException($"Booking with ID {id} not found");
        }

        if (booking.Status != BookingStatus.Confirmed)
        {
            throw new InvalidOperationException($"Cannot complete booking with status {booking.Status}");
        }

        // Update status to Completed
        booking.Status = BookingStatus.Completed;
        await _bookingRepository.UpdateAsync(booking);

        // Schedule the payout
        try
        {
            await SchedulePayoutAsync(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule payout for booking {BookingId}", id);
            // Log error but don't rollback - booking is completed
        }

        return booking;
    }
    
    public async Task<BookingEntity> CancelBookingAsync(int id)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
        {
            throw new KeyNotFoundException($"Booking with ID {id} not found");
        }

        if (booking.Status == BookingStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed booking");
        }

        booking.Status = BookingStatus.Cancelled;
        return await _bookingRepository.UpdateAsync(booking);
    }

    private async Task CapturePaymentAsync(BookingEntity booking)
    {
        var paymentApiUrl = _configuration["Services:PaymentAPI"] ?? "http://localhost:8085";
        var httpClient = _httpClientFactory.CreateClient();

        var captureRequest = new
        {
            BookingId = booking.Id,
            PaymentIntentId = booking.PaymentIntentId
        };

        var response = await httpClient.PostAsJsonAsync(
            $"{paymentApiUrl}/api/payment/capture",
            captureRequest);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Payment capture failed for booking {BookingId}. Status: {StatusCode}, Error: {Error}",
                booking.Id, response.StatusCode, errorContent);
            throw new HttpRequestException($"Payment capture failed: {response.StatusCode}");
        }

        _logger.LogInformation("Payment captured successfully for booking {BookingId}", booking.Id);
    }

    private async Task SchedulePayoutAsync(BookingEntity booking)
    {
        var paymentApiUrl = _configuration["Services:PaymentAPI"] ?? "http://localhost:8085";
        var httpClient = _httpClientFactory.CreateClient();

        var payoutRequest = new
        {
            BookingId = booking.Id,
            DriverId = booking.DriverId
        };

        var response = await httpClient.PostAsJsonAsync(
            $"{paymentApiUrl}/api/payment/payout",
            payoutRequest);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Payout scheduling failed for booking {BookingId}. Status: {StatusCode}, Error: {Error}",
                booking.Id, response.StatusCode, errorContent);
            throw new HttpRequestException($"Payout scheduling failed: {response.StatusCode}");
        }

        _logger.LogInformation("Payout scheduled successfully for booking {BookingId}", booking.Id);
    }
}

