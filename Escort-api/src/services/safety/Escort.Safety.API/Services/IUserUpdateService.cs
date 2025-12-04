namespace Escort.Safety.API.Services
{
    public interface IUserUpdateService
    {
        Task UpdateUserVerificationStatusAsync(string userId, bool isVerified);
    }
}

