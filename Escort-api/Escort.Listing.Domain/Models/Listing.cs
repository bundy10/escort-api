namespace Escort.Listing.Domain.Models;

public class Listing : BaseDomainModel
{
    public bool Listed { get; set; }
    public ListingDetails ListingDetails { get; set; }
    public int UserId { get; set; }

    public Listing(ListingDetails listingDetails, int userId)
    {
        Listed = true;
        ListingDetails = listingDetails;
        UserId = userId;
    }
}