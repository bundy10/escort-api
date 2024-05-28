using Escort.Listing.Domain.Models;

namespace Escort.Listing.API.DTO;

public class ListingPostPutDto
{
    public bool Listed { get; set; }
    public ListingDetails ListingDetails { get; set; }
    public int UserId { get; set; }
}