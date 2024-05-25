using Escort.User.Domain.Models;

namespace Escort.User.API.DTO;

public class UserGetDTO : UserPostPutDto
{
    public int Id { get; set; }
    
}