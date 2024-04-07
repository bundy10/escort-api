using Escort.User.API.Controllers;
using Escort.User.API.DTO;
using Escort.User.Application.Repositories;
using Escort.User.Domain.Models;
using Escort.User.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Escort.User.Test.Escort.User.API.test.Controller.test;

public class UserControllerTest
{
    private Mock<IUserRepository> _userRepositoryMock;
    private UserController _userController;
    private List<Domain.Models.User> _users;

    public UserControllerTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userController = new UserController(_userRepositoryMock.Object);
        _users = new List<Domain.Models.User>
        {
            new Domain.Models.User(new UserContactDetails
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "test@gmail.com",
                PhoneNumber = "1234567890"
            }, new UserVerificationDetails()),
            new Domain.Models.User(new UserContactDetails
            {
                FirstName = "James",
                LastName = "uncanny",
                Email = "test2@gmail.com",
                PhoneNumber = "1234567832"
            }, new UserVerificationDetails())
        };
    }

    [Fact]
    public async Task GetAllUsers_ReturnsOkObjectResult()
    {
        _userRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(_users);
        var result = await _userController.GetAllUsers();
        Assert.IsType<OkObjectResult>(result);
        var okObjectResult = result as OkObjectResult;
        Assert.NotNull(okObjectResult);
    }
    
    [Fact]
    public async Task GetUserById_ReturnsOkObjectResult()
    {
        var user = _users.First();
        _userRepositoryMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
        var result = await _userController.GetUserById(user.Id);
        Assert.IsType<OkObjectResult>(result);
        var okObjectResult = result as OkObjectResult;
        Assert.NotNull(okObjectResult);
    }
    
    [Fact]
    public async Task CreateUser_ReturnsCreatedAtActionResult()
    {
        var user = _users.First();
        var userPostPutDto = user.ToDto();
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Domain.Models.User>())).ReturnsAsync(user);
        
        var result = await _userController.CreateUser(userPostPutDto);
        
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<UserGetDTO>(createdAtActionResult.Value);
        Assert.Equal(user.Id, returnValue.Id);
    }
    
    [Fact]
    public async Task UpdateUser_ReturnsOkObjectResult()
    {
        var user = _users.First();
        var userPostPutDto = user.ToDto();
        _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Domain.Models.User>())).ReturnsAsync(user);
        
        var result = await _userController.UpdateUser(user.Id, userPostPutDto);
        
        var actionResult = Assert.IsType<ActionResult<UserGetDTO>>(result);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = Assert.IsType<UserGetDTO>(okObjectResult.Value);
        Assert.Equal(user.Id, returnValue.Id);
    }

    [Fact]
    public async Task DeleteUser_ReturnsOkObjectResult_WhenUserExists()
    {
        var id = Guid.NewGuid();
        var user = _users.First();
        _userRepositoryMock.Setup(x => x.DeleteAsync(id)).ReturnsAsync(user);
        
        var result = await _userController.DeleteUser(id);
        
        var actionResult = Assert.IsType<ActionResult<UserGetDTO>>(result);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = Assert.IsType<UserGetDTO>(okObjectResult.Value);
        Assert.Equal(user.Id, returnValue.Id);
    }
    
    [Fact]
    public async Task DeleteUser_ReturnsNotFoundResult_WhenUserDoesNotExist()
    {
        var id = Guid.NewGuid();
        _userRepositoryMock.Setup(x => x.DeleteAsync(id)).Throws<ModelNotFoundException>();
        
        var result = await _userController.DeleteUser(id);
        
        Assert.IsType<NotFoundResult>(result.Result);
    }
    
    [Fact]
    public async Task GetUserWithListings_ReturnsOkObjectResult_WhenUserExists()
    {
        var id = Guid.NewGuid();
        var user = _users.First();
        _userRepositoryMock.Setup(x => x.GetUserWithListingsAsync(id)).ReturnsAsync(user);
        
        var result = await _userController.GetUserWithListings(id);
        
        var okObjectResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserGetDTO>(okObjectResult.Value);
        Assert.Equal(user.Id, returnValue.Id);
    }
}