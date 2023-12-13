namespace Escort.User.Domain.Models;

public class UserContactDetails : BaseDomainModel
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    private UserVerificationDetails _verificationDetails = new();

    public UserContactDetails WithId()
    {
        this.Id = Guid.NewGuid();
        return this;
    }
}