using Escort.User.Domain.Models;

namespace Escort.User.API.DTO;

public static class UserMapper 
{
    public static UserGetDTO ToDto(this Domain.Models.UserContactDetails userContactDetails, UserVerificationDetails userVerificationDetails)
    {
        var user = new Domain.Models.User(userContactDetails, userVerificationDetails);
        return new UserGetDTO()
        {
            Id = user.Id,
            UserContactDetails = user.UserContactDetails,
            UserVerificationDetails = user.UserVerificationDetails
        };
        
    }
    public static Domain.Models.User ToDomain(this UserPostPutDto userPostPutDto)
    {
        return new Domain.Models.User(userPostPutDto.UserContactDetails, userPostPutDto.UserVerificationDetails).WithId();
    }
}