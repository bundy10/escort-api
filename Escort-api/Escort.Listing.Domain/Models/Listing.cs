namespace Escort.Listing.Domain.Models;

public class Listing : BaseDomainModel
{
    public bool Listed { get; set; }
    public ListingDetails ListingDetails { get; set; }
    public Guid UserId { get; set; }

    public Listing(ListingDetails listingDetails, Guid userId)
    {
        Listed = true;
        ListingDetails = listingDetails;
        UserId = userId;
    }
    
    public Listing WithId()
    {
        this.Id = Guid.NewGuid();
        return this;
    }
}