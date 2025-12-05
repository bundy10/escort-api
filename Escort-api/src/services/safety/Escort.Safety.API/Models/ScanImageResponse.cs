namespace Escort.Safety.API.Models;

public class ScanImageResponse
{
    public bool IsApproved { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? DetectedLabels { get; set; }
}

