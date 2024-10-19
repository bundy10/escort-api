using Escort.User.API.DTO;
using Escort.User.Application.Repositories;
using Escort.User.Infrastructure;
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
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return Ok(user.ToDto());
    }
    
    [HttpPost]
    public async Task<ActionResult<IEnumerable<UserGetDTO>>> CreateUser(UserPostPutDto userPostPutDto)
    {
        var user = await _userRepository.CreateAsync(userPostPutDto.ToDomain());
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user.ToDto());
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserGetDTO>> UpdateUser(int id, [FromBody] UserPostPutDto userPostPutDto)
    {
        {
            try
            {
                var user = userPostPutDto.ToDomain();
                user.Id = id;
                await _userRepository.UpdateAsync(user);
                var userGetDto = user.ToDto();

                return Ok(userGetDto);
            }
            catch (ModelNotFoundException)
            {
                return NotFound();
            }
        }
    }
    [HttpPost("authenticate")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        var user = await _userRepository.AuthenticateUserLoginAttempt(userLoginDto.Username, userLoginDto.Password);
        if (user != null)
        {
            return Ok();
        }
        return Unauthorized();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<UserGetDTO>> DeleteUser(int id)
    {
        try
        {
            var user = await _userRepository.DeleteAsync(id);
            return Ok(user.ToDto());
        }
        catch (ModelNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok();
    }
}