using System.Runtime.CompilerServices;

namespace Escort.User.Domain.Models;

public class User : BaseDomainModel
{
    public UserContactDetails? UserContactDetails { get; set; }
    public UserVerificationDetails? UserVerificationDetails {get; set;}

    public User(UserContactDetails userContactDetails, UserVerificationDetails userVerificationDetails)
    {
        UserContactDetails = userContactDetails;
        UserVerificationDetails = userVerificationDetails;
    }
}