namespace Escort.Client.Domain.Models;

public class ClientContactDetails: BaseDomainModel
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    
    public ClientContactDetails(string firstName, string lastName, string email, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
    }
    
    public ClientContactDetails WithId()
    {
        this.Id = Guid.NewGuid();
        return this;
    }
}