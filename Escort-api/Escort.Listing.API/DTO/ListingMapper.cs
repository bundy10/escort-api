namespace Escort.Listing.API.DTO;


public static class ListingMapper 
{
    public static ListingGetDTO ToDto(this Domain.Models.Listing listing)
    {
        return new ListingGetDTO()
        {   
            Id = listing.Id,
            Listed = listing.Listed,
            ListingDetails = listing.ListingDetails,
            UserId = listing.UserId
        };
    }
    
    public static Domain.Models.Listing ToDomain(this ListingPostPutDto listingPostPutDto)
    {
        return new Domain.Models.Listing(listingPostPutDto.ListingDetails, listingPostPutDto.UserId).WithId();
    }
}