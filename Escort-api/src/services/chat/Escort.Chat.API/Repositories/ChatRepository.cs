using Escort.Chat.API.Data;
using Escort.Chat.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Escort.Chat.API.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly ChatDbContext _context;
    private readonly ILogger<ChatRepository> _logger;

    public ChatRepository(ChatDbContext context, ILogger<ChatRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ChatMessage> SaveMessageAsync(ChatMessage message)
    {
        try
        {
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation(
                "Saved chat message {MessageId} for booking {BookingId} from sender {SenderId}",
                message.Id, message.BookingId, message.SenderId);
            
            return message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error saving chat message for booking {BookingId} from sender {SenderId}",
                message.BookingId, message.SenderId);
            throw;
        }
    }

    public async Task<IEnumerable<ChatMessage>> GetHistoryAsync(int bookingId)
    {
        try
        {
            var messages = await _context.ChatMessages
                .Where(m => m.BookingId == bookingId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
            
            _logger.LogInformation(
                "Retrieved {Count} messages for booking {BookingId}",
                messages.Count, bookingId);
            
            return messages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error retrieving chat history for booking {BookingId}",
                bookingId);
            throw;
        }
    }
}

