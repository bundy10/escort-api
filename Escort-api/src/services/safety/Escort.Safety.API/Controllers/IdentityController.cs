using Escort.Safety.API.Models;
using Escort.Safety.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Escort.Safety.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityVerificationService _identityVerificationService;
        private readonly ILogger<IdentityController> _logger;

        public IdentityController(
            IIdentityVerificationService identityVerificationService,
            ILogger<IdentityController> logger)
        {
            _identityVerificationService = identityVerificationService;
            _logger = logger;
        }

        /// <summary>
        /// Create a Stripe Identity verification session for a user
        /// </summary>
        /// <param name="request">Verification request containing user ID and optional return URL</param>
        /// <returns>Verification session URL for the user to complete identity verification</returns>
        [HttpPost("verify-identity")]
        [ProducesResponseType(typeof(VerifyIdentityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifyIdentity([FromBody] VerifyIdentityRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(new { error = "UserId is required" });
            }

            try
            {
                _logger.LogInformation("Creating identity verification session for user {UserId}", request.UserId);
                
                var response = await _identityVerificationService.CreateVerificationSessionAsync(request);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating verification session for user {UserId}", request.UserId);
                return StatusCode(500, new { error = "Failed to create verification session", detail = ex.Message });
            }
        }
    }
}

