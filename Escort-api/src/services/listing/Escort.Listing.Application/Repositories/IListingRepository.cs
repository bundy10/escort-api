namespace Escort.Listing.Application.Repositories;

public interface IListingRepository : IRepository<Domain.Models.Listing>
{
    Task<IEnumerable<Domain.Models.Listing>> SearchByLocationAsync(double latitude, double longitude, double radiusKm);
}