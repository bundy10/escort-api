namespace Escort.Safety.API.Services
{
    public class UserUpdateService : IUserUpdateService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserUpdateService> _logger;

        public UserUpdateService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<UserUpdateService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task UpdateUserVerificationStatusAsync(string userId, bool isVerified)
        {
            try
            {
                var userApiUrl = _configuration["UserApi:BaseUrl"] ?? "http://localhost:8080";
                var endpoint = $"{userApiUrl}/api/user/{userId}/verification-status";

                var payload = new { IsVerified = isVerified };
                var response = await _httpClient.PutAsJsonAsync(endpoint, payload);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully updated verification status for user {UserId} to {IsVerified}", 
                        userId, isVerified);
                }
                else
                {
                    _logger.LogWarning("Failed to update verification status for user {UserId}. Status: {StatusCode}", 
                        userId, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating verification status for user {UserId}", userId);
                // Don't throw - we don't want webhook to fail if user update fails
            }
        }
    }
}

