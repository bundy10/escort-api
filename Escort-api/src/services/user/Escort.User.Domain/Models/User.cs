using System.Runtime.CompilerServices;

namespace Escort.User.Domain.Models;

public class User : BaseDomainModel
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Password { get; set; }
    public required string UserName { get; set; }
    public required UserContactDetails UserContactDetails { get; set; }
    public UserVerificationDetails? UserVerificationDetails {get; set;}
    
    
}