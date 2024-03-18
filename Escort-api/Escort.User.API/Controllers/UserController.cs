using Escort.User.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Escort.User.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    
    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users.Select(user => user.ToDto()));
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return Ok(user.ToDto());
    }
}