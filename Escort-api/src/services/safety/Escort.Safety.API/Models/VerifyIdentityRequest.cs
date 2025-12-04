namespace Escort.Safety.API.Models
{
    public class VerifyIdentityRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }
    }
}

