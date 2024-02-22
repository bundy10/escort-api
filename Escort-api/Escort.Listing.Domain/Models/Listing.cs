namespace Escort.Listing.Domain.Models;

public class Listing : BaseDomainModel
{
    public bool Listed { get; set; }
    public ListingDetails ListingDetails { get; set; }

    public Listing(ListingDetails listingDetails)
    {
        Listed = true;
        ListingDetails = listingDetails;
    }
    
    public Listing WithId()
    {
        this.Id = Guid.NewGuid();
        return this;
    }
}