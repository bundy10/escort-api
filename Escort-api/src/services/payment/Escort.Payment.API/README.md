# Escort.Payment.API - Stripe Integration

## Overview
This API handles Stripe Connect Express account creation and onboarding for users.

## Endpoints

### POST /api/payment/create-account
Creates a Stripe Express account for a user (if they don't have one) and generates an onboarding link.

#### Request Body
```json
{
  "userId": 123,
  "returnUrl": "http://localhost:8085/payment/success",  // Optional
  "refreshUrl": "http://localhost:8085/payment/refresh"  // Optional
}
```

#### Response (Success - 200 OK)
```json
{
  "userId": 123,
  "url": "https://connect.stripe.com/setup/s/acct_xxxxx/yyyyy"
}
```

#### Response (Error - 400 Bad Request)
```json
{
  "error": "Valid UserId is required"
}
```

#### Response (Error - 500 Internal Server Error)
```json
{
  "error": "An unexpected error occurred"
}
```

## How It Works

### Account Creation Flow
1. **Check Existing Account**: The service checks if the user already has a Stripe account ID stored
2. **Create Express Account**: If no account exists, creates a new Stripe Express account with:
   - Account type: `express`
   - Country: `US`
   - Capabilities: Card payments and transfers
3. **Generate Account Link**: Creates an account onboarding link that expires after a short time
4. **Return URL**: Returns the onboarding URL to the frontend for user redirection

### Payment Authorization Flow
1. **Create PaymentIntent**: Creates a Stripe PaymentIntent with `CaptureMethod = manual`
2. **Transfer Group**: Associates the payment with a booking using `transfer_group`
3. **Client Secret**: Returns the client secret to the frontend for completing payment
4. **Frontend Integration**: Frontend uses the client secret with Stripe.js to collect payment method
5. **Authorization**: Payment is authorized but NOT captured (funds held, not transferred)
6. **Later Capture**: Payment can be captured after service completion (separate endpoint needed)

## Configuration

Update `appsettings.json` with your Stripe API keys:

```json
{
  "Stripe": {
    "SecretKey": "sk_test_your_actual_secret_key",
    "PublishableKey": "pk_test_your_actual_publishable_key"
  }
}
```
**`StripeAccountService`** handles:
## Architecture

### Dependency Injection
- `IStripeAccountService`: Interface for Stripe account operations

**`StripePaymentService`** handles:
- Creating PaymentIntents with manual capture
- Setting transfer groups for booking association
- Automatic payment methods configuration
- Converting amounts to cents (smallest currency unit)
- Error handling and logging
- `StripeAccountService`: Implementation using Stripe.net SDK
- Services are registered in `Program.cs` with scoped lifetime

### Service Layer
The `StripeAccountService` handles:
For production, you should:
1. Create a database table to store `UserId` -> `StripeAccountId` mappings
2. Create a database table to store `BookingId` -> `PaymentIntentId` mappings
3. Create a database table to store `BookingId` -> `TransferId` mappings for payouts
4. Use the actual user's email address from your user service
5. Configure webhook handlers for Stripe events (payment_intent.succeeded, transfer.created, etc.)
6. Add proper authentication/authorization to all endpoints
7. Consider the user's country for account creation and currency handling
8. Handle account status checking before creating new links
9. Implement payment capture endpoint for completing payments after service
10. Add payment cancellation/refund endpoints
11. Consider implementing idempotency keys for payment operations
12. Add proper currency validation and formatting
13. Verify connected account is active and can receive transfers
14. Implement retry logic for failed transfers
15. Store transfer metadata for audit trail and reconciliation
16. Consider platform fees (application_fee_amount) on PaymentIntent
17. Implement balance monitoring and alerts for low platform balance
12. Store payment metadata (customer info, service details, etc.)
- Request validation
- Default URL configuration
- Exception handling and proper HTTP responses
### Create Account
- Logging

## Production Considerations

⚠️ **Important**: The current implementation uses an in-memory dictionary to store user-account mappings. 

For production, you should:
1. Create a database table to store `UserId` -> `StripeAccountId` mappings
2. Use the actual user's email address from your user service
3. Configure webhook handlers for Stripe events
### Authorize Payment
```bash
curl -X POST http://localhost:5000/api/payment/authorize-payment \
  -H "Content-Type: application/json" \
  -d '{
    "bookingId": "booking_12345",
    "amount": 100.50,
    "currency": "usd"
  }'
```

4. Add proper authentication/authorization
5. Consider the user's country for account creation
6. Handle account status checking before creating new links

## Example Usage

```bash
curl -X POST http://localhost:5000/api/payment/create-account \
  -H "Content-Type: application/json" \
  -d '{
    "userId": 123,
    "returnUrl": "http://localhost:8085/payment/success",
    "refreshUrl": "http://localhost:8085/payment/refresh"
  }'
```

## Testing with Stripe Test Mode

When using Stripe test keys:
1. Use test mode API keys (sk_test_... and pk_test_...)
2. The onboarding link will work in test mode
3. You can complete onboarding with test data
4. No real money or bank accounts are needed

