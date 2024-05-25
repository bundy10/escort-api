using Escort.User.Domain.Models;

namespace Escort.User.API.DTO;

public class UserPostPutDto
{
    public UserContactDetails UserContactDetails { get; set; }
    public UserVerificationDetails UserVerificationDetails { get; set; }
}