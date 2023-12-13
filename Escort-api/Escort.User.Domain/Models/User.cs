using System.Runtime.CompilerServices;

namespace Escort.User.Domain.Models;

public class User : BaseDomainModel
{
    public UserContactDetails UserContactDetails { get; set; }
    public UserVerificationDetails UserVerificationDetails = new();

    public User(UserContactDetails userContactDetails)
    {
        UserContactDetails = userContactDetails;
    }
    public User WithId()
    {
        this.Id = Guid.NewGuid();
        return this;
    }
}