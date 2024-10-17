using Escort.User.Domain.Models;

namespace Escort.User.API.DTO;

public static class UserMapper 
{
    public static UserGetDTO ToDto(this Domain.Models.User user)
    {
        return new UserGetDTO()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserContactDetails = user.UserContactDetails,
            UserVerificationDetails = user.UserVerificationDetails,
        };
        
    }
    public static Domain.Models.User ToDomain(this UserPostPutDto userPostPutDto)
    {
        return new Domain.Models.User()
        {
            FirstName = userPostPutDto.FirstName,
            LastName = userPostPutDto.LastName,
            UserContactDetails = userPostPutDto.UserContactDetails,
            UserVerificationDetails = userPostPutDto.UserVerificationDetails ?? new UserVerificationDetails()
        };
    }
}