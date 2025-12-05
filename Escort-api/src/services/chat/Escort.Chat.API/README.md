# Escort.Chat.API - Real-time Chat with Database Persistence

## Overview
The Chat API provides real-time messaging capabilities using SignalR WebSockets with full database persistence. All chat messages are stored in PostgreSQL for history retrieval and auditing.

## Features
- **Real-time messaging** using SignalR
- **Database persistence** for all chat messages
- **Chat history retrieval** for any booking
- **Booking validation** - only authorized users can access chat rooms
- **Message timestamping** with UTC timestamps
- **Indexed queries** for fast message retrieval

## Architecture

### Database Layer
- **Entity**: `ChatMessage` (Id, BookingId, SenderId, MessageText, Timestamp)
- **DbContext**: `ChatDbContext` with PostgreSQL support
- **Repository Pattern**: `IChatRepository` / `ChatRepository`
- **Indexes**: On `BookingId` and `Timestamp` for optimal query performance

### SignalR Hub
- **ChatHub**: Main hub for real-time communication
- Messages are saved to database **before** broadcasting
- Includes authorization checks for all operations

## Database Setup

### Apply Migration
```bash
cd src/services/chat/Escort.Chat.API
dotnet ef database update
```

This creates the `ChatMessages` table with:
- Primary key on `Id`
- Index on `BookingId` for fast lookups
- Index on `Timestamp` for chronological queries

## API Endpoints

### SignalR Hub: `/chatHub`

#### Methods:

##### 1. JoinBookingGroup
Join a booking's chat room (required before sending/receiving messages).

```javascript
await connection.invoke("JoinBookingGroup", "123");
```

**Parameters:**
- `bookingId` (string): The booking ID to join

**Events:**
- Broadcasts `UserJoined` to all users in the booking group

---

##### 2. SendMessage
Send a message to a booking's chat room (saved to DB before broadcasting).

```javascript
await connection.invoke("SendMessage", "123", "Hello world!");
```

**Parameters:**
- `bookingId` (string): The booking ID
- `message` (string): The message text

**Behavior:**
1. Validates user is authorized for this booking
2. Saves message to database with timestamp
3. Broadcasts `ReceiveMessage` to all users in the booking group

**Event Data:**
```javascript
{
  "userId": "user123",
  "bookingId": "123",
  "message": "Hello world!",
  "timestamp": "2025-12-05T12:34:56.789Z"
}
```

---

##### 3. GetChatHistory
Retrieve all messages for a booking (ordered by timestamp).

```javascript
const history = await connection.invoke("GetChatHistory", "123");
```

**Parameters:**
- `bookingId` (string): The booking ID

**Returns:**
```javascript
[
  {
    "id": 1,
    "bookingId": 123,
    "senderId": "user123",
    "messageText": "Hello!",
    "timestamp": "2025-12-05T12:34:56.789Z"
  },
  {
    "id": 2,
    "bookingId": 123,
    "senderId": "user456",
    "messageText": "Hi there!",
    "timestamp": "2025-12-05T12:35:10.123Z"
  }
]
```

---

##### 4. LeaveBookingGroup
Leave a booking's chat room.

```javascript
await connection.invoke("LeaveBookingGroup", "123");
```

**Parameters:**
- `bookingId` (string): The booking ID to leave

**Events:**
- Broadcasts `UserLeft` to all users in the booking group

---

## Client Integration

### JavaScript/TypeScript Example

```javascript
import * as signalR from "@microsoft/signalr";

// Create connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:8087/chatHub")
    .withAutomaticReconnect()
    .build();

// Register event handlers
connection.on("ReceiveMessage", (data) => {
    console.log(`${data.userId}: ${data.message}`);
    // Update UI with new message
});

connection.on("UserJoined", (userId, bookingId) => {
    console.log(`${userId} joined booking ${bookingId}`);
});

connection.on("UserLeft", (userId, bookingId) => {
    console.log(`${userId} left booking ${bookingId}`);
});

// Start connection
await connection.start();

// Join a booking's chat room
await connection.invoke("JoinBookingGroup", "123");

// Load chat history
const history = await connection.invoke("GetChatHistory", "123");
history.forEach(msg => {
    console.log(`[${msg.timestamp}] ${msg.senderId}: ${msg.messageText}`);
});

// Send a message
await connection.invoke("SendMessage", "123", "Hello everyone!");

// Leave the chat room
await connection.invoke("LeaveBookingGroup", "123");

// Stop connection
await connection.stop();
```

### React Hook Example

```typescript
import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

export const useChat = (bookingId: string) => {
  const [connection, setConnection] = useState<signalR.HubConnection | null>(null);
  const [messages, setMessages] = useState<ChatMessage[]>([]);

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:8087/chatHub')
      .withAutomaticReconnect()
      .build();

    newConnection.on('ReceiveMessage', (data) => {
      setMessages(prev => [...prev, {
        id: 0, // Temporary ID
        bookingId: parseInt(data.bookingId),
        senderId: data.userId,
        messageText: data.message,
        timestamp: new Date(data.timestamp)
      }]);
    });

    newConnection.start()
      .then(() => {
        newConnection.invoke('JoinBookingGroup', bookingId);
        newConnection.invoke('GetChatHistory', bookingId)
          .then(history => setMessages(history));
      });

    setConnection(newConnection);

    return () => {
      if (connection) {
        connection.invoke('LeaveBookingGroup', bookingId);
        connection.stop();
      }
    };
  }, [bookingId]);

  const sendMessage = async (message: string) => {
    if (connection) {
      await connection.invoke('SendMessage', bookingId, message);
    }
  };

  return { messages, sendMessage };
};
```

## Configuration

### Connection String
The API uses the standard `ConnectionStrings__DefaultConnection` environment variable:

```bash
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=companiondb;Username=appuser;Password=123456789
```

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Services": {
    "BookingAPI": "http://localhost:8088"
  }
}
```

## Running the Service

### Development
```bash
cd src/services/chat/Escort.Chat.API
dotnet run
```

The service will be available at:
- HTTP: `http://localhost:8087`
- SignalR Hub: `http://localhost:8087/chatHub`
- Swagger UI: `http://localhost:8087/swagger`

### Docker
```bash
docker-compose up escort.chat.api
```

## Database Schema

### ChatMessages Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | int | PRIMARY KEY, AUTO INCREMENT | Unique message ID |
| BookingId | int | NOT NULL, INDEXED | The booking this message belongs to |
| SenderId | varchar(255) | NOT NULL | User ID who sent the message |
| MessageText | text | NOT NULL | The message content |
| Timestamp | timestamptz | NOT NULL, INDEXED | When the message was sent (UTC) |

### Indexes
- `IX_ChatMessages_BookingId`: Fast retrieval by booking
- `IX_ChatMessages_Timestamp`: Fast chronological sorting

## Security & Authorization

### Booking Validation
All chat operations validate that the user is part of the booking:
- Uses `IBookingValidationService` to check authorization
- Queries the Booking API to verify user has access
- Prevents unauthorized users from accessing chat rooms

### Best Practices
1. **Implement authentication**: Add JWT tokens to SignalR connections
2. **Rate limiting**: Prevent message spam
3. **Message validation**: Sanitize input to prevent XSS
4. **Connection ID tracking**: Use proper user IDs instead of connection IDs

## Error Handling

### HubException Scenarios
- **"User is not authorized"**: User doesn't have access to this booking
- **"Invalid booking ID format"**: BookingId must be a valid integer
- **"Failed to save message"**: Database error occurred

### Logging
All operations are logged with:
- Message save confirmations
- Error details for troubleshooting
- User actions for auditing

## Testing

### Manual Testing with Swagger
1. Navigate to `http://localhost:8087/swagger`
2. The SignalR hub is not testable via Swagger
3. Use the SignalR client or browser console for testing

### Browser Console Testing
```javascript
// Connect to the hub
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:8087/chatHub")
    .build();

connection.on("ReceiveMessage", console.log);
await connection.start();
await connection.invoke("JoinBookingGroup", "123");
await connection.invoke("SendMessage", "123", "Test message");
```

## Performance Considerations

### Database Queries
- Messages are retrieved with `OrderBy(Timestamp)` for chronological display
- Indexes on `BookingId` ensure fast lookups even with millions of messages
- Consider implementing pagination for very large chat histories

### SignalR Scalability
- Current implementation uses in-memory groups (single server)
- For multi-server deployments, configure a backplane:
  - Redis Backplane
  - Azure SignalR Service
  - SQL Server Backplane

### Recommended Optimizations
1. **Message pagination**: Limit history retrieval to last N messages
2. **Caching**: Cache recent messages in Redis
3. **Archive old messages**: Move messages older than 90 days to cold storage
4. **Connection pooling**: Already configured via EF Core

## Monitoring

### Key Metrics
- Active connections count
- Messages per second
- Database response time
- Failed message deliveries

### Logging Examples
```
info: Escort.Chat.API.Repositories.ChatRepository[0]
      Saved chat message 123 for booking 456 from sender user789

info: Escort.Chat.API.Hubs.ChatHub[0]
      Message saved to database for booking 456 from user user789

error: Escort.Chat.API.Repositories.ChatRepository[0]
      Error saving chat message for booking 456 from sender user789
      System.InvalidOperationException: Database connection failed
```

## Deployment Checklist

- [ ] Apply database migration: `dotnet ef database update`
- [ ] Configure connection string in environment variables
- [ ] Set up CORS for frontend domains
- [ ] Configure authentication/authorization
- [ ] Set up monitoring and logging
- [ ] Configure SignalR backplane for multiple servers
- [ ] Test with production load

## Future Enhancements

### Potential Features
- [ ] Message editing/deletion
- [ ] Read receipts
- [ ] Typing indicators
- [ ] File/image attachments
- [ ] Message reactions/emojis
- [ ] User mentions (@username)
- [ ] Message threading/replies
- [ ] Push notifications for offline users
- [ ] End-to-end encryption

## Dependencies
- ASP.NET Core 9.0
- SignalR 1.2.0
- Entity Framework Core 9.0.0
- Npgsql.EntityFrameworkCore.PostgreSQL 9.0.0
- DotNetEnv 3.1.1

## Support
For issues or questions, check the logs in:
- `/var/log/escort-chat-api/` (production)
- Console output (development)

