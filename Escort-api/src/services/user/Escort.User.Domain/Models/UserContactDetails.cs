namespace Escort.User.Domain.Models;

public class UserContactDetails : BaseDomainModel
{
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
}