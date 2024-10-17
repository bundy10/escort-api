using Escort.Listing.Domain.Models;

namespace Escort.Listing.API.DTO;

public class ListingPostPutDto
{
    public required bool Listed { get; set; }
    public required ListingDetails ListingDetails { get; set; }
    public required int UserId { get; set; }
}