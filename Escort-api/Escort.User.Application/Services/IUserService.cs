using Escort.User.Domain.Models;

namespace Escort.User.Application.Services;

public interface IUserService
{
    Task<Domain.Models.User> CreateUserAsync(Domain.Models.UserContactDetails userDetails, UserVerificationDetails userVerificationDetails);
    Task<IEnumerable<Domain.Models.User>> GetAllUsersAsync();
    Task<Domain.Models.User> GetUserByIdAsync(Guid id);
    Task<Domain.Models.User> UpdateUserAsync(UserContactDetails userDetails, UserVerificationDetails userVerificationDetails);
    Task<Domain.Models.User> DeleteUserAsync(Guid id);
    Task<Domain.Models.User> GetUserWithListingsAsync(Guid userId);
}