namespace Escort.User.Application.Repositories;

public interface IUserRepository : IRepository<Domain.Models.User>
{
    Task<Domain.Models.User> GetUserWithListingsAsync(Guid userId);
}