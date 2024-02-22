namespace Escort.Listing.Domain.Models;

public class ListingDetails : BaseDomainModel
{
    public ListingDetails(string title, string description, string location, DateTime date, string image, string category, string subCategory, string price)
    {
        Title = title;
        Description = description;
        Location = location;
        Date = date;
        Image = image;
        Category = category;
        SubCategory = subCategory;
        Price = price;
    }

    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public DateTime Date { get; set; }
    public string Image { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public string Price { get; set; }
}