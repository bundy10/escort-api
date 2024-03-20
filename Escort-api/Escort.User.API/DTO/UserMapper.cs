using Escort.Listing.API.DTO;
using Escort.User.Domain.Models;

namespace Escort.User.API.DTO;

public static class UserMapper 
{
    public static UserGetDTO ToDto(this Domain.Models.User user)
    {
        return new UserGetDTO()
        {
            Id = user.Id,
            UserContactDetails = user.UserContactDetails,
            UserVerificationDetails = user.UserVerificationDetails,
            Listings = user.Listings
        };
        
    }
    public static Domain.Models.User ToDomain(this UserPostPutDto userPostPutDto)
    {
        return new Domain.Models.User(userPostPutDto.UserContactDetails, userPostPutDto.UserVerificationDetails).WithId();
    }
}