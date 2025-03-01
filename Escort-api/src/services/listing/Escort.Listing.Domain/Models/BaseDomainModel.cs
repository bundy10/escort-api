namespace Escort.Listing.Domain.Models;

public class BaseDomainModel
{
    public int Id { get; set; }
    public required ListingDetails ListingDetails { get; set; }
    
}