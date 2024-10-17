namespace Escort.Listing.Domain.Models;

public class ListingDetails : BaseDomainModel
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Location { get; set; }
    public required DateTime Date { get; set; }
    public required string Image { get; set; }
    public required string Category { get; set; }
    public required string SubCategory { get; set; }
    public required string Price { get; set; }
}