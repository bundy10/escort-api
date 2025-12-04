using Microsoft.AspNetCore.SignalR;
using Escort.Chat.API.Services;

namespace Escort.Chat.API.Hubs;

public class ChatHub : Hub
{
    private readonly IBookingValidationService _bookingValidationService;

    public ChatHub(IBookingValidationService bookingValidationService)
    {
        _bookingValidationService = bookingValidationService;
    }

    public async Task JoinBookingGroup(string bookingId)
    {
        // Get the user ID from the connection context
        var userId = Context.UserIdentifier ?? Context.ConnectionId;

        // Validate that the user is part of the booking
        var isAuthorized = await _bookingValidationService.IsUserPartOfBooking(userId, bookingId);

        if (!isAuthorized)
        {
            throw new HubException("User is not authorized to join this booking group.");
        }

        // Add the user to the booking group
        await Groups.AddToGroupAsync(Context.ConnectionId, $"booking_{bookingId}");

        // Notify the group that a user has joined
        await Clients.Group($"booking_{bookingId}").SendAsync("UserJoined", userId, bookingId);
    }

    public async Task SendMessage(string bookingId, string message)
    {
        // Get the user ID from the connection context
        var userId = Context.UserIdentifier ?? Context.ConnectionId;

        // Validate that the user is part of the booking
        var isAuthorized = await _bookingValidationService.IsUserPartOfBooking(userId, bookingId);

        if (!isAuthorized)
        {
            throw new HubException("User is not authorized to send messages to this booking group.");
        }

        // Broadcast the message to all users in the booking group
        await Clients.Group($"booking_{bookingId}").SendAsync("ReceiveMessage", new
        {
            UserId = userId,
            BookingId = bookingId,
            Message = message,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task LeaveBookingGroup(string bookingId)
    {
        var userId = Context.UserIdentifier ?? Context.ConnectionId;
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"booking_{bookingId}");
        
        // Notify the group that a user has left
        await Clients.Group($"booking_{bookingId}").SendAsync("UserLeft", userId, bookingId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Handle cleanup when a user disconnects
        await base.OnDisconnectedAsync(exception);
    }
}

