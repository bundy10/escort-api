using Microsoft.AspNetCore.Mvc;
using Escort.Payment.API.Services;

namespace Escort.Payment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IStripeAccountService _stripeAccountService;
    private readonly IStripePaymentService _stripePaymentService;
    private readonly IPayoutService _payoutService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(
        IStripeAccountService stripeAccountService,
        IStripePaymentService stripePaymentService,
        IPayoutService payoutService,
        ILogger<PaymentController> logger)
    {
        _stripeAccountService = stripeAccountService;
        _stripePaymentService = stripePaymentService;
        _payoutService = payoutService;
        _logger = logger;
    }

    [HttpPost("create-account")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        if (request.UserId <= 0)
        {
            return BadRequest(new { error = "Valid UserId is required" });
        }

        // Default URLs if not provided
        var returnUrl = string.IsNullOrEmpty(request.ReturnUrl) 
            ? "http://localhost:8085/payment/success" 
            : request.ReturnUrl;
        
        var refreshUrl = string.IsNullOrEmpty(request.RefreshUrl) 
            ? "http://localhost:8085/payment/refresh" 
            : request.RefreshUrl;

        try
        {
            _logger.LogInformation("Creating Stripe account link for user {UserId}", request.UserId);

            // Create or retrieve Stripe Express account and generate account link
            var accountLinkUrl = await _stripeAccountService.CreateAccountLinkAsync(
                request.UserId, 
                returnUrl, 
                refreshUrl);

            _logger.LogInformation("Successfully created account link for user {UserId}", request.UserId);

            return Ok(new CreateAccountResponse
            {
                UserId = request.UserId,
                Url = accountLinkUrl
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to create account link for user {UserId}", request.UserId);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating account link for user {UserId}", request.UserId);
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }

    [HttpPost("authorize-payment")]
    public async Task<IActionResult> AuthorizePayment([FromBody] AuthorizePaymentRequest request)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.BookingId))
        {
            return BadRequest(new { error = "BookingId is required" });
        }

        if (request.Amount <= 0)
        {
            return BadRequest(new { error = "Amount must be greater than 0" });
        }

        if (string.IsNullOrWhiteSpace(request.Currency))
        {
            return BadRequest(new { error = "Currency is required" });
        }

        try
        {
            _logger.LogInformation(
                "Authorizing payment for booking {BookingId} with amount {Amount} {Currency}",
                request.BookingId, request.Amount, request.Currency);

            // Create PaymentIntent with manual capture
            var clientSecret = await _stripePaymentService.AuthorizePaymentAsync(
                request.BookingId,
                request.Amount,
                request.Currency);

            _logger.LogInformation("Successfully authorized payment for booking {BookingId}", request.BookingId);

            return Ok(new AuthorizePaymentResponse
            {
                BookingId = request.BookingId,
                ClientSecret = clientSecret,
                Amount = request.Amount,
                Currency = request.Currency
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to authorize payment for booking {BookingId}", request.BookingId);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error authorizing payment for booking {BookingId}", request.BookingId);
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }

    [HttpPost("capture")]
    public async Task<IActionResult> CapturePayment([FromBody] CapturePaymentRequest request)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.PaymentIntentId))
        {
            return BadRequest(new { error = "PaymentIntentId is required" });
        }

        try
        {
            _logger.LogInformation(
                "Capturing payment for booking {BookingId} with PaymentIntent {PaymentIntentId}",
                request.BookingId, request.PaymentIntentId);

            // Capture the previously authorized payment
            var paymentIntentId = await _stripePaymentService.CapturePaymentAsync(request.PaymentIntentId);

            _logger.LogInformation(
                "Successfully captured payment for booking {BookingId}. PaymentIntent ID: {PaymentIntentId}",
                request.BookingId, paymentIntentId);

            return Ok(new CapturePaymentResponse
            {
                BookingId = request.BookingId,
                PaymentIntentId = paymentIntentId,
                Status = "captured"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to capture payment for booking {BookingId}", request.BookingId);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error capturing payment for booking {BookingId}", request.BookingId);
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }

    [HttpPost("payout")]
    public async Task<IActionResult> SchedulePayout([FromBody] SchedulePayoutRequest request)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.BookingId))
        {
            return BadRequest(new { error = "BookingId is required" });
        }

        if (request.DriverId <= 0)
        {
            return BadRequest(new { error = "Valid DriverId is required" });
        }

        try
        {
            _logger.LogInformation(
                "Scheduling payout for booking {BookingId} to driver {DriverId}",
                request.BookingId, request.DriverId);

            // Get the driver's Stripe account and transfer funds
            // Note: This is a simplified version - you'd need to retrieve the actual amount and account
            var destinationAccountId = $"acct_driver_{request.DriverId}"; // This should come from your user/driver database
            var amount = request.Amount > 0 ? request.Amount : 1000; // Default or retrieve from booking

            var transferId = await _payoutService.ReleaseFundsAsync(
                request.BookingId,
                destinationAccountId,
                amount);

            _logger.LogInformation(
                "Successfully scheduled payout for booking {BookingId}. Transfer ID: {TransferId}",
                request.BookingId, transferId);

            return Ok(new SchedulePayoutResponse
            {
                BookingId = request.BookingId,
                TransferId = transferId,
                DriverId = request.DriverId,
                Status = "scheduled"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to schedule payout for booking {BookingId}", request.BookingId);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error scheduling payout for booking {BookingId}", request.BookingId);
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }

    [HttpPost("release-funds")]
    public async Task<IActionResult> ReleaseFunds([FromBody] ReleaseFundsRequest request)
    {
        // Validate request
        if (string.IsNullOrWhiteSpace(request.BookingId))
        {
            return BadRequest(new { error = "BookingId is required" });
        }

        if (string.IsNullOrWhiteSpace(request.DestinationAccountId))
        {
            return BadRequest(new { error = "DestinationAccountId is required" });
        }

        if (request.Amount <= 0)
        {
            return BadRequest(new { error = "Amount must be greater than 0" });
        }

        try
        {
            _logger.LogInformation(
                "Releasing funds for booking {BookingId} to account {AccountId} with amount {Amount}",
                request.BookingId, request.DestinationAccountId, request.Amount);

            // Release funds to connected account
            var transferId = await _payoutService.ReleaseFundsAsync(
                request.BookingId,
                request.DestinationAccountId,
                request.Amount);

            _logger.LogInformation(
                "Successfully released funds for booking {BookingId}. Transfer ID: {TransferId}",
                request.BookingId, transferId);

            return Ok(new ReleaseFundsResponse
            {
                BookingId = request.BookingId,
                TransferId = transferId,
                Amount = request.Amount,
                DestinationAccountId = request.DestinationAccountId
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request parameters for releasing funds");
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to release funds for booking {BookingId}", request.BookingId);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error releasing funds for booking {BookingId}", request.BookingId);
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }
}

public class CreateAccountRequest
{
    public int UserId { get; set; }
    public string? ReturnUrl { get; set; }
    public string? RefreshUrl { get; set; }
}

public class CreateAccountResponse
{
    public int UserId { get; set; }
    public string Url { get; set; } = string.Empty;
}

public class AuthorizePaymentRequest
{
    public string BookingId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "usd";
}

public class AuthorizePaymentResponse
{
    public string BookingId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
}

public class ReleaseFundsRequest
{
    public string BookingId { get; set; } = string.Empty;
    public string DestinationAccountId { get; set; } = string.Empty;
    public long Amount { get; set; }
}

public class ReleaseFundsResponse
{
    public string BookingId { get; set; } = string.Empty;
    public string TransferId { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string DestinationAccountId { get; set; } = string.Empty;
}

public class CapturePaymentRequest
{
    public string BookingId { get; set; } = string.Empty;
    public string PaymentIntentId { get; set; } = string.Empty;
}

public class CapturePaymentResponse
{
    public string BookingId { get; set; } = string.Empty;
    public string PaymentIntentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class SchedulePayoutRequest
{
    public string BookingId { get; set; } = string.Empty;
    public int DriverId { get; set; }
    public long Amount { get; set; }
}

public class SchedulePayoutResponse
{
    public string BookingId { get; set; } = string.Empty;
    public string TransferId { get; set; } = string.Empty;
    public int DriverId { get; set; }
    public string Status { get; set; } = string.Empty;
}

