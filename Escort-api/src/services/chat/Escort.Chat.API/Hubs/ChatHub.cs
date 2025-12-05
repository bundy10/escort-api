using Microsoft.AspNetCore.SignalR;
using Escort.Chat.API.Services;
using Escort.Chat.API.Repositories;
using Escort.Chat.API.Models;

namespace Escort.Chat.API.Hubs;

public class ChatHub : Hub
{
    private readonly IBookingValidationService _bookingValidationService;
    private readonly IChatRepository _chatRepository;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        IBookingValidationService bookingValidationService,
        IChatRepository chatRepository,
        ILogger<ChatHub> logger)
    {
        _bookingValidationService = bookingValidationService;
        _chatRepository = chatRepository;
        _logger = logger;
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

        // Parse bookingId to int
        if (!int.TryParse(bookingId, out int bookingIdInt))
        {
            throw new HubException("Invalid booking ID format.");
        }

        var timestamp = DateTime.UtcNow;

        // Save message to database
        try
        {
            var chatMessage = new ChatMessage
            {
                BookingId = bookingIdInt,
                SenderId = userId,
                MessageText = message,
                Timestamp = timestamp
            };

            await _chatRepository.SaveMessageAsync(chatMessage);
            
            _logger.LogInformation(
                "Message saved to database for booking {BookingId} from user {UserId}",
                bookingId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to save message to database for booking {BookingId}",
                bookingId);
            throw new HubException("Failed to save message to database.");
        }

        // Broadcast the message to all users in the booking group
        await Clients.Group($"booking_{bookingId}").SendAsync("ReceiveMessage", new
        {
            UserId = userId,
            BookingId = bookingId,
            Message = message,
            Timestamp = timestamp
        });
    }
    
    public async Task<IEnumerable<ChatMessage>> GetChatHistory(string bookingId)
    {
        // Get the user ID from the connection context
        var userId = Context.UserIdentifier ?? Context.ConnectionId;

        // Validate that the user is part of the booking
        var isAuthorized = await _bookingValidationService.IsUserPartOfBooking(userId, bookingId);

        if (!isAuthorized)
        {
            throw new HubException("User is not authorized to view this booking's chat history.");
        }

        // Parse bookingId to int
        if (!int.TryParse(bookingId, out int bookingIdInt))
        {
            throw new HubException("Invalid booking ID format.");
        }

        return await _chatRepository.GetHistoryAsync(bookingIdInt);
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

