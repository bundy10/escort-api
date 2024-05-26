using Escort.Listing.Domain.Models;

namespace Escort.Listing.Application.Services;

public interface IListingService
{
    Task<IEnumerable<Domain.Models.Listing>> GetAllAsync();
    Task<Domain.Models.Listing> GetByIdAsync(int id);
    Task<Domain.Models.Listing> CreateAsync(ListingDetails listingDetails, int userId);
    Task<Domain.Models.Listing> UpdateAsync(ListingDetails listingDetails, int userId);
    Task<Domain.Models.Listing> DeleteAsync(int id);
}