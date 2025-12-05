using Escort.Chat.API.Models;

namespace Escort.Chat.API.Repositories;

public interface IChatRepository
{
    Task<ChatMessage> SaveMessageAsync(ChatMessage message);
    Task<IEnumerable<ChatMessage>> GetHistoryAsync(int bookingId);
}

