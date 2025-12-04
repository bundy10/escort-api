# Frontend Integration Guide for Payment Authorization

## Overview
This guide shows how to integrate the `/authorize-payment` endpoint with your frontend using Stripe.js.

## Step 1: Backend - Authorize Payment

When a user initiates a booking, call the backend to create a PaymentIntent:

```javascript
async function authorizePayment(bookingId, amount, currency = 'usd') {
  const response = await fetch('http://localhost:5000/api/payment/authorize-payment', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      bookingId: bookingId,
      amount: amount,
      currency: currency
    })
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.error || 'Failed to authorize payment');
  }

  return await response.json();
}
```

## Step 2: Frontend - Collect Payment Method

Use the returned `clientSecret` with Stripe.js to collect payment information:

```javascript
// Initialize Stripe.js with your publishable key
const stripe = Stripe('pk_test_your_publishable_key');

// Create Elements instance
const elements = stripe.elements();

// Create and mount the Payment Element
const paymentElement = elements.create('payment');
paymentElement.mount('#payment-element');

// Handle form submission
async function handleSubmit(event) {
  event.preventDefault();

  try {
    // Get the client secret from backend
    const { clientSecret, bookingId } = await authorizePayment('booking_12345', 100.50, 'usd');

    // Confirm the payment
    const { error, paymentIntent } = await stripe.confirmPayment({
      elements,
      clientSecret: clientSecret,
      confirmParams: {
        return_url: 'http://localhost:8085/payment/complete',
      },
      redirect: 'if_required' // Don't redirect if payment succeeds
    });

    if (error) {
      // Show error to customer
      console.error('Payment failed:', error.message);
      showError(error.message);
    } else if (paymentIntent.status === 'requires_capture') {
      // Payment authorized successfully!
      console.log('Payment authorized:', paymentIntent.id);
      // Redirect to booking confirmation page
      window.location.href = `/booking/${bookingId}/confirmed`;
    }
  } catch (err) {
    console.error('Error:', err);
    showError(err.message);
  }
}
```

## Step 3: HTML Structure

```html
<form id="payment-form">
  <div id="payment-element">
    <!-- Stripe.js will create form elements here -->
  </div>
  
  <button type="submit">Authorize Payment</button>
  
  <div id="error-message" style="display: none; color: red;"></div>
</form>

<script src="https://js.stripe.com/v3/"></script>
<script src="your-payment-script.js"></script>
```

## Step 4: Payment States

The PaymentIntent will have different statuses:

- **`requires_payment_method`**: Waiting for payment method
- **`requires_confirmation`**: Payment method attached, needs confirmation
- **`requires_action`**: Requires additional authentication (3D Secure)
- **`requires_capture`**: ✅ **Payment authorized! Ready for capture later**
- **`succeeded`**: Payment captured (if auto-capture was enabled)
- **`canceled`**: Payment was canceled

## Step 5: Capture Payment (Backend - Future Implementation)

After the service is completed, you'll need to capture the payment:

```csharp
// Future endpoint to implement
[HttpPost("capture-payment")]
public async Task<IActionResult> CapturePayment([FromBody] CapturePaymentRequest request)
{
    // Capture the authorized payment
    var paymentIntentService = new PaymentIntentService();
    var paymentIntent = await paymentIntentService.CaptureAsync(request.PaymentIntentId);
    
    return Ok(new { status = paymentIntent.Status });
}
```

## Important Notes

1. **Authorization Hold**: The payment is held but not captured. Most banks hold authorizations for 7 days
2. **Capture Later**: You must capture before the hold expires, or the funds will be released
3. **Amount Flexibility**: You can capture less than authorized (e.g., if service was partial)
4. **Security**: Always validate amounts on the backend, never trust frontend input
5. **Error Handling**: Handle network errors, insufficient funds, declined cards gracefully
6. **3D Secure**: Some cards require additional authentication - Stripe.js handles this automatically

## Testing with Stripe Test Cards

Use these test card numbers:

- **Success**: `4242 4242 4242 4242`
- **Requires Authentication**: `4000 0025 0000 3155`
- **Declined**: `4000 0000 0000 0002`
- **Insufficient Funds**: `4000 0000 0000 9995`

Use any future expiry date and any 3-digit CVC.

## Webhook Integration (Recommended)

Set up webhooks to handle payment events:

```javascript
// Stripe webhook endpoint (backend)
app.post('/webhook', async (req, res) => {
  const sig = req.headers['stripe-signature'];
  
  try {
    const event = stripe.webhooks.constructEvent(req.body, sig, endpointSecret);
    
    switch (event.type) {
      case 'payment_intent.succeeded':
        // Payment captured successfully
        break;
      case 'payment_intent.payment_failed':
        // Payment failed
        break;
      case 'payment_intent.canceled':
        // Payment canceled
        break;
    }
    
    res.json({ received: true });
  } catch (err) {
    res.status(400).send(`Webhook Error: ${err.message}`);
  }
});
```

