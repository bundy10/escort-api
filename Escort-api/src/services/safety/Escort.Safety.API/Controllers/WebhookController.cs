using Escort.Safety.API.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Escort.Safety.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserUpdateService _userUpdateService;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            IConfiguration configuration,
            IUserUpdateService userUpdateService,
            ILogger<WebhookController> logger)
        {
            _configuration = configuration;
            _userUpdateService = userUpdateService;
            _logger = logger;
        }

        /// <summary>
        /// Webhook endpoint for Stripe Identity events
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _configuration["Stripe:WebhookSecret"]
                );

                _logger.LogInformation("Received Stripe webhook event: {EventType}", stripeEvent.Type);

                // Handle the identity verification completed event
                if (stripeEvent.Type == "identity.verification_session.verified")
                {
                    var verificationSession = stripeEvent.Data.Object as Stripe.Identity.VerificationSession;
                    
                    if (verificationSession != null && verificationSession.Metadata.ContainsKey("user_id"))
                    {
                        var userId = verificationSession.Metadata["user_id"];
                        
                        _logger.LogInformation(
                            "Identity verification completed for user {UserId}. Session: {SessionId}, Status: {Status}",
                            userId, verificationSession.Id, verificationSession.Status);

                        // Update user verification status
                        await _userUpdateService.UpdateUserVerificationStatusAsync(userId, true);
                    }
                    else
                    {
                        _logger.LogWarning("Verification session does not contain user_id in metadata");
                    }
                }
                // Handle failed verification
                else if (stripeEvent.Type == "identity.verification_session.requires_input")
                {
                    var verificationSession = stripeEvent.Data.Object as Stripe.Identity.VerificationSession;
                    
                    if (verificationSession != null && verificationSession.Metadata.ContainsKey("user_id"))
                    {
                        var userId = verificationSession.Metadata["user_id"];
                        
                        _logger.LogWarning(
                            "Identity verification requires input for user {UserId}. Session: {SessionId}",
                            userId, verificationSession.Id);
                    }
                }
                else if (stripeEvent.Type == "identity.verification_session.canceled")
                {
                    var verificationSession = stripeEvent.Data.Object as Stripe.Identity.VerificationSession;
                    
                    if (verificationSession != null && verificationSession.Metadata.ContainsKey("user_id"))
                    {
                        var userId = verificationSession.Metadata["user_id"];
                        
                        _logger.LogWarning(
                            "Identity verification canceled for user {UserId}. Session: {SessionId}",
                            userId, verificationSession.Id);
                        
                        // Update user verification status to false
                        await _userUpdateService.UpdateUserVerificationStatusAsync(userId, false);
                    }
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook signature verification failed");
                return BadRequest(new { error = "Invalid signature" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook");
                return StatusCode(500, new { error = "Webhook processing failed" });
            }
        }
    }
}

