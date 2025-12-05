# Chat API Database Integration - Quick Start

## ✅ What Was Done

### 1. Added NuGet Packages
- `Microsoft.EntityFrameworkCore` (9.0.0)
- `Npgsql.EntityFrameworkCore.PostgreSQL` (9.0.0)
- `Microsoft.EntityFrameworkCore.Design` (9.0.0)

### 2. Created Database Layer
- **Models/ChatMessage.cs** - Entity with Id, BookingId, SenderId, MessageText, Timestamp
- **Data/ChatDbContext.cs** - EF Core DbContext with ChatMessages DbSet
- **Data/ChatDbContextFactory.cs** - Design-time factory for migrations
- **Repositories/IChatRepository.cs** - Interface with SaveMessageAsync and GetHistoryAsync
- **Repositories/ChatRepository.cs** - Implementation with full logging

### 3. Updated ChatHub
- Added `IChatRepository` dependency injection
- Modified `SendMessage` to save messages to database **before** broadcasting
- Added `GetChatHistory` method to retrieve message history
- Added comprehensive error handling and logging

### 4. Updated Program.cs
- Configured `ChatDbContext` with PostgreSQL
- Registered `IChatRepository` as scoped service
- Uses `ConnectionStrings__DefaultConnection` from .env file

### 5. Created Database Migration
- Migration: `20251204224433_AddChatMessages`
- Creates `ChatMessages` table with indexes on BookingId and Timestamp

## 🚀 Next Steps

### 1. Apply the Database Migration
```bash
cd src/services/chat/Escort.Chat.API
dotnet ef database update
```

This will create the `ChatMessages` table in your `companiondb` database.

### 2. Run the Chat API
```bash
cd src/services/chat/Escort.Chat.API
dotnet run
```

The service will start on `http://localhost:8087` with the SignalR hub at `/chatHub`.

### 3. Test the Changes

**Connect to the hub:**
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:8087/chatHub")
    .withAutomaticReconnect()
    .build();

await connection.start();
```

**Join a booking and send a message:**
```javascript
// Join booking chat room
await connection.invoke("JoinBookingGroup", "123");

// Send a message (now saves to DB!)
await connection.invoke("SendMessage", "123", "Hello world!");

// Retrieve chat history
const history = await connection.invoke("GetChatHistory", "123");
console.log(history);
```

## 📊 Database Schema

The migration creates this table:

```sql
CREATE TABLE "ChatMessages" (
    "Id" SERIAL PRIMARY KEY,
    "BookingId" INTEGER NOT NULL,
    "SenderId" VARCHAR(255) NOT NULL,
    "MessageText" TEXT NOT NULL,
    "Timestamp" TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE INDEX "IX_ChatMessages_BookingId" ON "ChatMessages" ("BookingId");
CREATE INDEX "IX_ChatMessages_Timestamp" ON "ChatMessages" ("Timestamp");
```

## 🔄 Message Flow

### Before (Memory Only):
1. User sends message via SignalR
2. Message broadcasted to all connected users
3. **Lost when server restarts**

### After (With Database):
1. User sends message via SignalR
2. **Message saved to database** ✅
3. Message broadcasted to all connected users
4. **History retrievable anytime** ✅
5. **Persists across server restarts** ✅

## 📝 New Features

### 1. Persistent Chat History
```javascript
// Get all messages for a booking
const messages = await connection.invoke("GetChatHistory", "123");

// Returns:
[
  {
    id: 1,
    bookingId: 123,
    senderId: "user123",
    messageText: "Hello!",
    timestamp: "2025-12-05T12:34:56.789Z"
  },
  // ... more messages
]
```

### 2. Automatic Message Saving
Every message sent via `SendMessage` is automatically:
- Validated for authorization
- Saved to database with UTC timestamp
- Broadcasted to all users in the booking group
- Logged for monitoring

### 3. Enhanced Error Handling
- Database save failures return proper exceptions
- Invalid booking IDs are caught early
- Authorization failures prevent unauthorized access

## 🔍 Verification

After applying the migration, verify the table exists:

```bash
psql -h localhost -U appuser -d companiondb -c "\dt ChatMessages"
```

Check that messages are being saved:

```bash
psql -h localhost -U appuser -d companiondb -c "SELECT * FROM \"ChatMessages\" LIMIT 10;"
```

## 📚 Documentation

Full documentation available in:
- `src/services/chat/Escort.Chat.API/README.md` - Complete API documentation

## ⚠️ Important Notes

1. **Connection String**: Uses `ConnectionStrings__DefaultConnection` from your `.env` file
2. **Migration Required**: Must run `dotnet ef database update` before the API will work
3. **Booking Validation**: Existing `IBookingValidationService` is still used for authorization
4. **Indexes**: Created on BookingId and Timestamp for optimal query performance

## 🎯 What This Enables

- **Chat history**: Users can see previous messages when rejoining
- **Auditing**: All messages are stored for compliance/moderation
- **Analytics**: Query message patterns, user activity, etc.
- **Recovery**: Messages survive server crashes/restarts
- **Reporting**: Generate reports on chat usage

## 🚨 Build Status

✅ **Build Successful** - All code compiles without errors
✅ **Migration Created** - Ready to apply to database
✅ **No Breaking Changes** - Existing SignalR functionality unchanged

