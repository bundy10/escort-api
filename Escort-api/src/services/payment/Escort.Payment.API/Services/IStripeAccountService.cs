namespace Escort.Payment.API.Services;

public interface IStripeAccountService
{
    Task<string> CreateAccountLinkAsync(int userId, string returnUrl, string refreshUrl);
}

