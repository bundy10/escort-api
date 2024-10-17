namespace Escort.Listing.Domain.Models;

public class ListingDetails : BaseDomainModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public DateTime Date { get; set; }
    public string Image { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public string Price { get; set; }
}