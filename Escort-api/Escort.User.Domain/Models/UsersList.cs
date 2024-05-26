namespace Escort.User.Domain.Models;

public class UsersList : BaseDomainModel
{
    public required ICollection<User> Users { get; set; }
}