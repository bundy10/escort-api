using Escort.User.Domain.Models;

namespace Escort.User.API.DTO;

public class UserPostPutDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required UserContactDetails UserContactDetails { get; set; }
    public UserVerificationDetails? UserVerificationDetails { get; set; }
}