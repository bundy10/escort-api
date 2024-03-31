using Escort.User.API.Controllers;
using Escort.User.API.DTO;
using Escort.User.Application.Repositories;
using Escort.User.Domain.Models;
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
}