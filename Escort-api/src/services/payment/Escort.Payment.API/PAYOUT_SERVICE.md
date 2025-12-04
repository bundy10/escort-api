# PayoutService Documentation

## Overview
The `PayoutService` handles the release of funds from the platform account to connected accounts (drivers/service providers) after a service is completed. It uses Stripe Transfers API to move funds while maintaining the same `transfer_group` as the original PaymentIntent for complete transaction tracking.

## Key Features

### 1. **Balance Validation**
Before attempting any transfer, the service checks the platform's available balance to prevent failed transfers due to insufficient funds.

### 2. **Transfer Group Matching**
Uses the same `transfer_group` (BookingId) that was set on the PaymentIntent during authorization. This allows you to:
- Track all financial transactions related to a booking
- Generate reports showing payment flow from customer to driver
- Handle disputes with complete transaction history

### 3. **Comprehensive Error Handling**
Handles various Stripe error scenarios:
- **Insufficient Funds**: Platform balance too low
- **Invalid Account**: Destination account doesn't exist or can't receive funds
- **Invalid Transfer Group**: Booking ID format issues
- **General Stripe Errors**: Network issues, API changes, etc.

### 4. **Detailed Logging**
Every operation is logged with relevant context:
- Transfer attempts with booking and account details
- Balance checks and validation results
- Success confirmations with transfer IDs
- Error details for debugging

## Implementation Details

### Service Interface
```csharp
public interface IPayoutService
{
    Task<string> ReleaseFundsAsync(string bookingId, string destinationAccountId, long amount);
}
```

### Method Parameters
- **bookingId**: The booking identifier used as transfer_group
- **destinationAccountId**: Stripe connected account ID (format: `acct_xxxxx`)
- **amount**: Amount in cents (e.g., 10050 = $100.50)

### Return Value
Returns the Stripe Transfer ID (format: `tr_xxxxx`) which can be stored for:
- Transaction reconciliation
- Dispute handling
- Financial reporting
- Audit trails

## Usage Flow

### Complete Payment Flow
```
1. Customer books service
   ↓
2. POST /authorize-payment
   - Creates PaymentIntent with transfer_group = bookingId
   - Returns clientSecret to frontend
   ↓
3. Frontend completes payment with Stripe.js
   - Payment is AUTHORIZED but NOT CAPTURED
   ↓
4. Service is completed
   ↓
5. Capture the payment (future endpoint)
   - Funds move from customer to platform account
   ↓
6. POST /release-funds
   - Transfers funds from platform to driver
   - Uses same transfer_group for tracking
   ↓
7. Driver receives payout
```

## Error Scenarios

### 1. Insufficient Platform Balance
```json
{
  "error": "Insufficient platform balance. Available: $50.00, Required: $100.50"
}
```
**Solution**: Ensure platform account has sufficient captured funds before releasing payouts.

### 2. Invalid Destination Account
```json
{
  "error": "Invalid destination account: acct_1234567890"
}
```
**Solution**: Verify the driver has completed Stripe onboarding and their account is active.

### 3. Payment Not Captured Yet
If you try to transfer funds before capturing the payment, you'll get an insufficient balance error.
**Solution**: Implement and call a payment capture endpoint first.

## Code Example

### Backend - Release Funds
```csharp
var transferId = await _payoutService.ReleaseFundsAsync(
    bookingId: "booking_12345",
    destinationAccountId: "acct_1234567890", 
    amount: 10050  // $100.50 in cents
);
```

### API Call
```bash
curl -X POST http://localhost:5000/api/payment/release-funds \
  -H "Content-Type: application/json" \
  -d '{
    "bookingId": "booking_12345",
    "destinationAccountId": "acct_1234567890",
    "amount": 10050
  }'
```

### Response
```json
{
  "bookingId": "booking_12345",
  "transferId": "tr_1234567890abcdef",
  "amount": 10050,
  "destinationAccountId": "acct_1234567890"
}
```

## Stripe Dashboard

### Viewing Transfers
1. Go to Stripe Dashboard → **Payments** → **Transfers**
2. Search by transfer_group (booking ID) to see all related transactions
3. View transfer details including:
   - Source (platform account)
   - Destination (connected account)
   - Amount
   - Status
   - Creation date

### Tracking Transaction Flow
With matching transfer_groups, you can:
```
PaymentIntent (pi_xxxxx) → transfer_group: "booking_12345"
    ↓
Transfer (tr_xxxxx) → transfer_group: "booking_12345"
```

## Best Practices

### 1. Store Transfer IDs
Save the returned transfer ID in your database:
```sql
CREATE TABLE Payouts (
    Id INT PRIMARY KEY,
    BookingId VARCHAR(50),
    TransferId VARCHAR(50),
    DestinationAccountId VARCHAR(50),
    Amount BIGINT,
    CreatedAt DATETIME
);
```

### 2. Implement Idempotency
Prevent duplicate transfers:
```csharp
// Check if transfer already exists for this booking
var existingPayout = await _dbContext.Payouts
    .FirstOrDefaultAsync(p => p.BookingId == bookingId);
    
if (existingPayout != null)
{
    return existingPayout.TransferId; // Already processed
}
```

### 3. Use Webhooks
Listen for transfer events:
```csharp
switch (stripeEvent.Type)
{
    case "transfer.created":
        // Transfer initiated
        break;
    case "transfer.paid":
        // Transfer completed
        break;
    case "transfer.failed":
        // Transfer failed - investigate and retry
        break;
}
```

### 4. Consider Platform Fees
Optionally take a platform fee:
```csharp
// In PaymentIntent creation
var paymentIntentOptions = new PaymentIntentCreateOptions
{
    Amount = 10050,
    Currency = "usd",
    ApplicationFeeAmount = 1005, // 10% platform fee
    TransferGroup = bookingId,
    // ... other options
};

// Then transfer driver's portion
var driverAmount = 9045; // 90% of payment
await _payoutService.ReleaseFundsAsync(bookingId, driverAccountId, driverAmount);
```

### 5. Monitor Platform Balance
Set up alerts for low balance:
```csharp
var balance = await _balanceService.GetAsync();
var availableUsd = balance.Available.FirstOrDefault(b => b.Currency == "usd");

if (availableUsd?.Amount < minimumThreshold)
{
    // Send alert to administrators
    await _notificationService.SendLowBalanceAlert(availableUsd.Amount);
}
```

## Testing

### Test Mode
Use Stripe test keys to simulate transfers:
1. Create test connected accounts
2. Authorize test payments
3. Transfer test funds
4. All operations work identically to production

### Verify Transfer Success
```csharp
var transferService = new TransferService();
var transfer = await transferService.GetAsync(transferId);

Console.WriteLine($"Status: {transfer.Status}");
Console.WriteLine($"Amount: ${transfer.Amount / 100.0:F2}");
Console.WriteLine($"Destination: {transfer.Destination}");
```

## Common Issues

### Issue: "Insufficient funds in platform account"
**Cause**: Payment hasn't been captured yet, or was refunded.
**Fix**: Capture the payment before releasing funds.

### Issue: "Invalid destination account"
**Cause**: Driver hasn't completed Stripe onboarding.
**Fix**: Ensure driver completes account verification via `/create-account` endpoint.

### Issue: "Transfer already exists"
**Cause**: Attempting duplicate transfer for same booking.
**Fix**: Check database before creating transfer, return existing transfer ID.

## Security Considerations

1. **Validate Booking Completion**: Only release funds after service is verified complete
2. **Authenticate Requests**: Ensure only authorized services can trigger payouts
3. **Verify Amounts**: Compare requested amount against booking amount in database
4. **Log All Attempts**: Maintain audit trail of all payout attempts
5. **Rate Limiting**: Prevent abuse with rate limits on payout endpoint

## Next Steps

Consider implementing:
1. **Scheduled Payouts**: Batch multiple transfers to reduce fees
2. **Payout Status Tracking**: Store and display transfer status to drivers
3. **Failed Transfer Retry**: Automatic retry logic with exponential backoff
4. **Multi-Currency Support**: Handle different currencies per region
5. **Payout Reports**: Generate financial reports for drivers and administrators

