namespace Escort.User.Domain.Models;

public class UsersDomainModel : BaseDomainModel
{
    public required ICollection<User> Users { get; set; }

    public UsersDomainModel WithId()
    {
        this.Id = Guid.NewGuid();
        return this;
    }
}