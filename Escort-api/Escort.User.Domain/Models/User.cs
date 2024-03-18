using System.Runtime.CompilerServices;

namespace Escort.User.Domain.Models;

public class User : BaseDomainModel
{
    public UserContactDetails UserContactDetails { get; set; }
    public UserVerificationDetails UserVerificationDetails {get; set;}
    public ICollection<Listing.Domain.Models.Listing> Listings { get; set; }

    public User(UserContactDetails userContactDetails, UserVerificationDetails userVerificationDetails)
    {
        UserContactDetails = userContactDetails;
        UserVerificationDetails = userVerificationDetails;
    }
    public User WithId()
    {
        this.Id = Guid.NewGuid();
        return this;
    }
    public User WithListings(ICollection<Listing.Domain.Models.Listing> listings)
    {
        Listings = listings;
        return this;
    }
}