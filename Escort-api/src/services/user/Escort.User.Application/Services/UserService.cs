using Escort.User.Application.Repositories;
using Escort.User.Domain.Models;

namespace Escort.User.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Domain.Models.User> CreateUserAsync(UserContactDetails userDetails, UserVerificationDetails userVerificationDetails, string firstName, string lastName)
    {
        var user = new Domain.Models.User
        {
            UserContactDetails = userDetails,
            UserVerificationDetails = userVerificationDetails,
            FirstName = firstName,
            LastName = lastName
        };
        return await _userRepository.CreateAsync(user);
    }
    
    public async Task<IEnumerable<Domain.Models.User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }
    
    public async Task<Domain.Models.User> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
    
    public async Task<Domain.Models.User> UpdateUserAsync(UserContactDetails userDetails, UserVerificationDetails userVerificationDetails, string firstName, string lastName)
    {
        var user = new Domain.Models.User
        {
            UserContactDetails = userDetails,
            UserVerificationDetails = userVerificationDetails,
            FirstName = firstName,
            LastName = lastName
        };
        return await _userRepository.UpdateAsync(user);
    }
    
    public async Task<Domain.Models.User> DeleteUserAsync(int id)
    {
        return await _userRepository.DeleteAsync(id);
    }
}