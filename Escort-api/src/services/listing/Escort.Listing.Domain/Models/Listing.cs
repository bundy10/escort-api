using NetTopologySuite.Geometries;

namespace Escort.Listing.Domain.Models;

public class Listing : BaseDomainModel
{
    public required bool Listed { get; set; }
    public required int UserId { get; set; }
    public Point? Location { get; set; }
}