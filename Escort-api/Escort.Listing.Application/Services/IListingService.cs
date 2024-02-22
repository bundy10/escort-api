namespace Escort.Listing.Application.Services;

public interface IListingService
{
    Task<IEnumerable<Domain.Models.Listing>> GetAllAsync();
    Task<Domain.Models.Listing> GetByIdAsync(Guid id);
    Task<Domain.Models.Listing> CreateAsync(Domain.Models.ListingDetails listingDetails);
    Task<Domain.Models.Listing> UpdateAsync(Domain.Models.ListingDetails listingDetails);
    Task<Domain.Models.Listing> DeleteAsync(Guid id);
}