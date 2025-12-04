using Escort.Safety.API.Models;

namespace Escort.Safety.API.Services
{
    public interface IIdentityVerificationService
    {
        Task<VerifyIdentityResponse> CreateVerificationSessionAsync(VerifyIdentityRequest request);
    }
}

