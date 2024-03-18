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

    public async Task<Domain.Models.User> CreateUserAsync(UserContactDetails userDetails, UserVerificationDetails userVerificationDetails)
    {
        var user = new Domain.Models.User(userDetails, userVerificationDetails);
        return await _userRepository.CreateAsync(user);
    }
    
    public async Task<IEnumerable<Domain.Models.User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }
    
    public async Task<Domain.Models.User> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
    
    public async Task<Domain.Models.User> UpdateUserAsync(UserContactDetails userDetails, UserVerificationDetails userVerificationDetails)
    {
        var user = new Domain.Models.User(userDetails, userVerificationDetails);
        return await _userRepository.UpdateAsync(user);
    }
    
    public async Task<Domain.Models.User> DeleteUserAsync(Guid id)
    {
        return await _userRepository.DeleteAsync(id);
    }
    
    public async Task<Domain.Models.User> GetUserWithListingsAsync(Guid userId)
    {
        return await _userRepository.GetUserWithListingsAsync(userId);
    }
}