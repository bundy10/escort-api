namespace Escort.User.Domain.Models;

public class BaseDomainModel
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public int Id { get; set; }  
}

