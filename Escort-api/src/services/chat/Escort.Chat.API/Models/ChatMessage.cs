namespace Escort.Chat.API.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string MessageText { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

