using Escort.Booking.Domain.Models;

namespace Escort.Booking.API.DTO;

public static class BookingMapper
{
    public static BookingGetDTO ToGetDTO(Domain.Models.Booking booking)
    {
        return new BookingGetDTO
        {
            Id = booking.Id,
            UserId = booking.UserId,
            ClientId = booking.ClientId,
            DriverId = booking.DriverId,
            ListingId = booking.ListingId,
            Date = booking.Date,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            BookingTime = booking.BookingTime,
            Status = booking.Status,
            PaymentIntentId = booking.PaymentIntentId
        };
    }

    public static Domain.Models.Booking ToModel(BookingPostPutDto dto)
    {
        return new Domain.Models.Booking
        {
            UserId = dto.UserId,
            ClientId = dto.ClientId,
            DriverId = dto.DriverId,
            ListingId = dto.ListingId,
            Date = dto.Date,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            PaymentIntentId = dto.PaymentIntentId
        };
    }
}

