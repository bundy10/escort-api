namespace Escort.Safety.API.Models
{
    public class VerifyIdentityResponse
    {
        public string SessionId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}

