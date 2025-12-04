using Escort.Listing.Application.Repositories;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Escort.Listing.Infrastructure.Repositories;

public class ListingRepository : BaseRepository<Domain.Models.Listing>, IListingRepository
{
    private readonly DbContext _context;
    
    public ListingRepository(DbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Domain.Models.Listing>> SearchByLocationAsync(double latitude, double longitude, double radiusKm)
    {
        // Create a point from the search coordinates (SRID 4326 = WGS84)
        var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var searchPoint = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
        
        // Convert radius from km to meters (PostGIS ST_Distance uses meters for geography)
        var radiusMeters = radiusKm * 1000;
        
        // Query listings within the specified radius
        // Using ST_DWithin for efficient spatial index usage
        var listings = await _context.Set<Domain.Models.Listing>()
            .Include(l => l.ListingDetails)
            .Where(l => l.Location != null && l.Location.IsWithinDistance(searchPoint, radiusMeters))
            .OrderBy(l => l.Location!.Distance(searchPoint))
            .ToListAsync();
        
        return listings;
    }
}