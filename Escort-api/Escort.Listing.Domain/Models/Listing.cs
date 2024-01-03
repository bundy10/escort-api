namespace Escort.Listing.Domain.Models;

public class Listing : BaseDomainModel
{
    public required bool Listed { get; set; }
    public required User.Domain.Models.User ListingOwner;

    public Listing(User.Domain.Models.User user)
    {
        ListingOwner = user;
    }
    public Listing WithId()
    {
        this.Id = Guid.NewGuid();
        return this;
    }
}