using Escort.User.Domain.Models;

namespace Escort.User.Application.Services;

public interface IUserService
{
    Task<Domain.Models.User> CreateUserAsync(Domain.Models.User user);
    Task<IEnumerable<Domain.Models.User>> GetAllUsersAsync();
    Task<Domain.Models.User> GetUserByIdAsync(int id);
    Task<Domain.Models.User> UpdateUserAsync(Domain.Models.User user);
    Task<Domain.Models.User> DeleteUserAsync(int id);
    Task<Domain.Models.User?> AuthenticateUserLoginAttempt(string username, string password);
}