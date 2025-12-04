# Quick Start Guide - Escort.Worker.Payout

## What Has Been Created

✅ **Escort.Worker.Payout** - A console application configured as a Hosted Service
✅ **Database Migration** - Added `PayoutDueAt` and `PayoutProcessed` fields to Booking model
✅ **Automated Processing** - Runs every hour to process pending payouts
✅ **Payment API Integration** - Calls the release-funds endpoint
✅ **Comprehensive Logging** - Detailed logs for monitoring

## Files Created/Modified

### New Files:
- `src/workers/Escort.Worker.Payout/Program.cs` - Main entry point with DI configuration
- `src/workers/Escort.Worker.Payout/Workers/PayoutWorker.cs` - Background service that runs every hour
- `src/workers/Escort.Worker.Payout/Services/PayoutProcessingService.cs` - Business logic for payout processing
- `src/workers/Escort.Worker.Payout/Services/IPayoutProcessingService.cs` - Service interface
- `src/workers/Escort.Worker.Payout/DTOs/ReleaseFundsRequest.cs` - Payment API request DTO
- `src/workers/Escort.Worker.Payout/DTOs/ReleaseFundsResponse.cs` - Payment API response DTO
- `src/workers/Escort.Worker.Payout/appsettings.json` - Configuration file
- `src/workers/Escort.Worker.Payout/README.md` - Comprehensive documentation

### Modified Files:
- `src/services/booking/Escort.Booking.Domain/Models/Booking.cs` - Added payout fields
- `src/services/booking/Escort.Booking.Infrastructure/DBcontext/BookingDbContext.cs` - Updated EF configuration
- `src/services/booking/Escort.Booking.Infrastructure/Migrations/[timestamp]_AddPayoutFields.cs` - New migration

## Next Steps

### 1. Configure Database Connection
Edit `src/workers/Escort.Worker.Payout/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "BookingDatabase": "Host=localhost;Database=your_booking_db;Username=your_user;Password=your_password"
  }
}
```

### 2. Apply Database Migration
```bash
cd src/services/booking/Escort.Booking.Infrastructure
dotnet ef database update --startup-project ../Escort.Booking.API/Escort.Booking.API.csproj
```

### 3. Update Your Booking API
When marking a booking as completed, set the payout due date:
```csharp
booking.Status = BookingStatus.Completed;
booking.PayoutDueAt = DateTime.UtcNow.AddDays(7); // 7-day hold period
await _context.SaveChangesAsync();
```

### 4. Run the Worker
```bash
cd src/workers/Escort.Worker.Payout
dotnet run
```

## Configuration Options

### Change Processing Interval
Edit `appsettings.json`:
```json
{
  "PayoutWorker": {
    "IntervalInMinutes": 30  // Run every 30 minutes instead of 60
  }
}
```

### Change Payment API URL
```json
{
  "PaymentAPI": {
    "BaseUrl": "http://your-payment-api:8085",
    "ReleaseFundsEndpoint": "/api/Payment/release-funds"
  }
}
```

## Important TODOs

### 1. Implement Payout Amount Calculation
Location: `Services/PayoutProcessingService.cs` - `CalculatePayoutAmount()` method

Currently returns a placeholder $100. You need to implement:
- Fetch listing hourly rate
- Calculate booking duration
- Apply platform commission
- Return amount in cents

### 2. Get Driver Stripe Account ID
Location: `Services/PayoutProcessingService.cs` - `ProcessBookingPayoutAsync()` method

Currently uses placeholder `acct_driver_{driverId}`. You need to:
- Query User/Driver database for Stripe account ID
- Or add HTTP call to User API
- Handle missing/incomplete Stripe onboarding

### 3. Test with Real Data
1. Create a test booking
2. Mark it as Completed with PayoutDueAt set to the past
3. Ensure PaymentIntentId is set
4. Run the worker and verify logs

## Testing the Worker

### View Logs
The worker provides comprehensive logging:
```
Payout Worker started. Processing interval: 60 minutes
Starting payout processing at 12/05/2025 08:37:22 +00:00
Found 3 bookings pending payout
Processing payout for booking 123 for driver 456
Calling Payment API at http://localhost:8085/api/Payment/release-funds for booking 123
Payment API returned success for booking 123. Transfer ID: tr_xxxxx
Successfully processed payout for booking 123
Completed payout processing at 12/05/2025 08:37:25 +00:00
Payout Worker completed. Next run at: 12/05/2025 09:37:25 +00:00
```

### Manual Testing
You can test the processing logic without waiting for the hourly schedule by temporarily reducing the interval to 1 minute in appsettings.json.

## Production Deployment

### As a Linux Systemd Service
Create `/etc/systemd/system/escort-payout-worker.service`:
```ini
[Unit]
Description=Escort Payout Worker
After=network.target

[Service]
Type=notify
WorkingDirectory=/app/Escort.Worker.Payout
ExecStart=/usr/bin/dotnet /app/Escort.Worker.Payout/Escort.Worker.Payout.dll
Restart=always
RestartSec=10
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

Then:
```bash
sudo systemctl daemon-reload
sudo systemctl enable escort-payout-worker
sudo systemctl start escort-payout-worker
sudo systemctl status escort-payout-worker
```

## Troubleshooting

### Worker Not Finding Bookings
- Check database connection string
- Verify bookings have `Status = Completed`
- Verify `PayoutDueAt` is set and in the past
- Verify `PayoutProcessed = false`

### Payment API Errors
- Verify Payment API is running
- Check Payment API URL in configuration
- Ensure bookings have valid `PaymentIntentId`
- Check driver has completed Stripe onboarding

### Build Errors
Already resolved! The project builds successfully. If you encounter errors after pulling changes, run:
```bash
dotnet restore
dotnet build
```

## Support

See the full README.md for detailed documentation on:
- Architecture and design
- Error handling
- Monitoring recommendations
- Production considerations

