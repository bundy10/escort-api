using Escort.Safety.API.Models;
using Stripe.Identity;

namespace Escort.Safety.API.Services
{
    public class IdentityVerificationService : IIdentityVerificationService
    {
        private readonly VerificationSessionService _verificationSessionService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IdentityVerificationService> _logger;

        public IdentityVerificationService(
            IConfiguration configuration,
            ILogger<IdentityVerificationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _verificationSessionService = new VerificationSessionService();
        }

        public async Task<VerifyIdentityResponse> CreateVerificationSessionAsync(VerifyIdentityRequest request)
        {
            try
            {
                var options = new VerificationSessionCreateOptions
                {
                    Type = "document",
                    Metadata = new Dictionary<string, string>
                    {
                        { "user_id", request.UserId }
                    },
                    Options = new VerificationSessionOptionsOptions
                    {
                        Document = new VerificationSessionOptionsDocumentOptions
                        {
                            // Require a document with a face
                            RequireLiveCapture = true,
                            RequireIdNumber = true,
                            RequireMatchingSelfie = true
                        }
                    }
                };

                // Add return URL if provided
                if (!string.IsNullOrEmpty(request.ReturnUrl))
                {
                    options.ReturnUrl = request.ReturnUrl;
                }

                var session = await _verificationSessionService.CreateAsync(options);

                _logger.LogInformation("Created verification session {SessionId} for user {UserId}", 
                    session.Id, request.UserId);

                return new VerifyIdentityResponse
                {
                    SessionId = session.Id,
                    Url = session.Url,
                    Status = session.Status
                };
            }
            catch (Stripe.StripeException ex)
            {
                _logger.LogError(ex, "Stripe error creating verification session for user {UserId}", request.UserId);
                throw new Exception($"Failed to create verification session: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating verification session for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}

