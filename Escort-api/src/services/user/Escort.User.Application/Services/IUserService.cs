using Escort.User.Domain.Models;

namespace Escort.User.Application.Services;

public interface IUserService
{
    Task<Domain.Models.User> CreateUserAsync(Domain.Models.UserContactDetails userDetails, UserVerificationDetails userVerificationDetails, string firstName, string lastName);
    Task<IEnumerable<Domain.Models.User>> GetAllUsersAsync();
    Task<Domain.Models.User> GetUserByIdAsync(int id);
    Task<Domain.Models.User> UpdateUserAsync(UserContactDetails userDetails, UserVerificationDetails userVerificationDetails, string firstName, string lastName);
    Task<Domain.Models.User> DeleteUserAsync(int id);
}