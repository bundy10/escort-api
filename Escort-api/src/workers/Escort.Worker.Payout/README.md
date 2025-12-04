# Escort.Worker.Payout

## Overview
A console application configured as a Hosted Service that processes pending payouts for completed bookings. The worker runs every hour (configurable) and automatically releases funds to drivers through the Payment API.

## Features
- **Automated Payout Processing**: Runs on a configurable schedule (default: every hour)
- **Intelligent Query**: Finds bookings where:
  - Status == Completed
  - PayoutDueAt < DateTime.UtcNow
  - PayoutProcessed == false
- **Payment API Integration**: Calls Escort.Payment.API to release funds
- **Database Updates**: Marks bookings as PayoutProcessed after successful payout
- **Comprehensive Logging**: Detailed logs for monitoring and debugging
- **Error Resilience**: Continues processing other bookings if one fails

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "BookingDatabase": "Host=localhost;Database=booking_db;Username=user;Password=pass"
  },
  "PaymentAPI": {
    "BaseUrl": "http://localhost:8085",
    "ReleaseFundsEndpoint": "/api/Payment/release-funds"
  },
  "PayoutWorker": {
    "IntervalInMinutes": 60,
    "BatchSize": 100
  }
}
```

### Environment Variables
You can also configure using environment variables:
- `ConnectionStrings__BookingDatabase`
- `PaymentAPI__BaseUrl`
- `PayoutWorker__IntervalInMinutes`

## Database Migration

Before running the worker, you need to apply the database migration that adds the payout fields:

```bash
cd src/services/booking/Escort.Booking.Infrastructure
dotnet ef database update --startup-project ../Escort.Booking.API/Escort.Booking.API.csproj
```

This adds the following fields to the Bookings table:
- `PayoutDueAt` (DateTime, nullable): When the payout should be processed
- `PayoutProcessed` (bool, default false): Whether the payout has been processed

## Running the Worker

### Development
```bash
cd src/workers/Escort.Worker.Payout
dotnet run
```

### Production
```bash
cd src/workers/Escort.Worker.Payout
dotnet build -c Release
dotnet run -c Release
```

### As a Windows Service or Linux Daemon
You can deploy this as a system service using tools like:
- Windows: `sc.exe` or NSSM
- Linux: systemd

## How It Works

1. **Scheduled Execution**: The worker runs every hour (configurable)
2. **Query Bookings**: Finds all bookings that need payout processing
3. **For Each Booking**:
   - Validates booking has required data (DriverId, PaymentIntentId)
   - Calculates payout amount (TODO: implement proper calculation)
   - Gets driver's Stripe account ID (TODO: integrate with User/Driver service)
   - Calls Payment API to release funds
   - Marks booking as PayoutProcessed = true on success
4. **Error Handling**: Logs errors and continues with next booking
5. **Wait**: Sleeps until next scheduled execution

## TODO / Implementation Notes

### 1. Payout Amount Calculation
The current implementation uses a placeholder amount. You need to implement proper calculation in `PayoutProcessingService.CalculatePayoutAmount()`:

```csharp
private long CalculatePayoutAmount(Booking booking)
{
    // Calculate based on:
    // - Booking duration (EndTime - StartTime)
    // - Listing hourly rate
    // - Platform commission (e.g., 20%)
    // - Any additional fees
    
    // Example:
    // var duration = booking.EndTime - booking.StartTime;
    // var hours = duration.TotalHours;
    // var totalAmount = hours * listingHourlyRate;
    // var platformFee = totalAmount * 0.20m;
    // var driverPayout = totalAmount - platformFee;
    // return (long)(driverPayout * 100); // Convert to cents
}
```

### 2. Driver Stripe Account ID
Currently uses a placeholder `acct_driver_{driverId}`. You need to:
- Store Stripe account IDs in your User/Driver database
- Retrieve the actual account ID before calling Payment API
- Handle cases where driver hasn't completed Stripe onboarding

Options:
- Add HTTP call to User/Driver API
- Add direct database query to User database
- Include Stripe account ID in Booking record

### 3. Setting PayoutDueAt
When a booking is marked as Completed, you need to set the `PayoutDueAt` field. This should be done in your Booking API:

```csharp
booking.Status = BookingStatus.Completed;
booking.PayoutDueAt = DateTime.UtcNow.AddDays(7); // 7-day hold period
await _context.SaveChangesAsync();
```

## Monitoring

### Logs
The worker provides detailed logging:
- Startup/shutdown events
- Number of bookings found
- Processing status for each booking
- API call results
- Errors and exceptions

### Recommended Monitoring
- Set up log aggregation (e.g., ELK, Seq, Application Insights)
- Alert on repeated failures
- Monitor processing duration
- Track payout success rate

## Dependencies
- Escort.Booking.Domain
- Escort.Booking.Infrastructure
- Microsoft.Extensions.Hosting
- Entity Framework Core
- Npgsql (PostgreSQL provider)
- HttpClient for Payment API calls

## Error Scenarios

### Database Connection Failed
Check connection string configuration and ensure database is accessible.

### Payment API Unavailable
Worker will log error and retry on next scheduled run. Consider implementing retry logic within the same run.

### Insufficient Funds
Payment API will return error. Check platform Stripe account balance.

### Invalid Driver Account
Ensure drivers have completed Stripe Connect onboarding before bookings are completed.

## Production Considerations

1. **Connection Pooling**: Already configured via EF Core
2. **Retry Logic**: Consider adding Polly for transient failures
3. **Idempotency**: Bookings won't be reprocessed due to PayoutProcessed flag
4. **Monitoring**: Set up health checks and alerting
5. **Graceful Shutdown**: Worker respects CancellationToken
6. **Scaling**: Run single instance to avoid duplicate processing
7. **Batch Size**: Currently processes all pending payouts; consider batching for large volumes

