using Escort.Listing.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Escort.Listing.Infrastructure.Repositories;

public class ListingRepository : BaseRepository<Domain.Models.Listing>, IListingRepository
{
    public ListingRepository(DbContext context) : base(context)
    {
    }
}