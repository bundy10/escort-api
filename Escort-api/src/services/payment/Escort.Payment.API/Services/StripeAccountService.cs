using Stripe;

namespace Escort.Payment.API.Services;

public class StripeAccountService : IStripeAccountService
{
    private readonly AccountService _accountService;
    private readonly AccountLinkService _accountLinkService;
    private readonly ILogger<StripeAccountService> _logger;
    
    // In-memory store for demo purposes - in production, use a database
    private static readonly Dictionary<int, string> UserStripeAccounts = new();

    public StripeAccountService(ILogger<StripeAccountService> logger)
    {
        _accountService = new AccountService();
        _accountLinkService = new AccountLinkService();
        _logger = logger;
    }

    public async Task<string> CreateAccountLinkAsync(int userId, string returnUrl, string refreshUrl)
    {
        try
        {
            string stripeAccountId;

            // Check if user already has a Stripe account
            if (UserStripeAccounts.TryGetValue(userId, out var existingAccountId))
            {
                _logger.LogInformation("User {UserId} already has Stripe account {AccountId}", userId, existingAccountId);
                stripeAccountId = existingAccountId;
            }
            else
            {
                // Create a new Stripe Express account
                var accountOptions = new AccountCreateOptions
                {
                    Type = "express",
                    Country = "US",
                    Email = $"user{userId}@example.com", // In production, use actual user email
                    Capabilities = new AccountCapabilitiesOptions
                    {
                        CardPayments = new AccountCapabilitiesCardPaymentsOptions
                        {
                            Requested = true
                        },
                        Transfers = new AccountCapabilitiesTransfersOptions
                        {
                            Requested = true
                        }
                    }
                };

                var account = await _accountService.CreateAsync(accountOptions);
                stripeAccountId = account.Id;
                
                // Store the account ID (in production, save to database)
                UserStripeAccounts[userId] = stripeAccountId;
                
                _logger.LogInformation("Created Stripe Express account {AccountId} for user {UserId}", stripeAccountId, userId);
            }

            // Create an Account Link for onboarding
            var accountLinkOptions = new AccountLinkCreateOptions
            {
                Account = stripeAccountId,
                RefreshUrl = refreshUrl,
                ReturnUrl = returnUrl,
                Type = "account_onboarding"
            };

            var accountLink = await _accountLinkService.CreateAsync(accountLinkOptions);
            
            _logger.LogInformation("Created account link for user {UserId}: {Url}", userId, accountLink.Url);

            return accountLink.Url;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error occurred while creating account link for user {UserId}", userId);
            throw new InvalidOperationException($"Failed to create Stripe account link: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while creating account link for user {UserId}", userId);
            throw;
        }
    }
}

