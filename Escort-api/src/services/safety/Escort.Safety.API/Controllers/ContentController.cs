using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Escort.Safety.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Escort.Safety.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentController : ControllerBase
{
    private readonly ILogger<ContentController> _logger;
    private readonly IAmazonRekognition _rekognitionClient;

    public ContentController(
        ILogger<ContentController> logger,
        IAmazonRekognition rekognitionClient)
    {
        _logger = logger;
        _rekognitionClient = rekognitionClient;
    }

    /// <summary>
    /// Scan an image for inappropriate content using AWS Rekognition
    /// </summary>
    /// <param name="file">The image file to scan</param>
    /// <returns>Approval status with detected labels</returns>
    [HttpPost("scan-image")]
    [ProducesResponseType(typeof(ScanImageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ScanImageResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ScanImage(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ScanImageResponse
            {
                IsApproved = false,
                Message = "No file provided"
            });
        }

        // Validate file is an image
        var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
        if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
        {
            return BadRequest(new ScanImageResponse
            {
                IsApproved = false,
                Message = "Invalid file type. Only JPEG and PNG images are allowed."
            });
        }

        // Limit file size to 5MB
        if (file.Length > 5 * 1024 * 1024)
        {
            return BadRequest(new ScanImageResponse
            {
                IsApproved = false,
                Message = "File size exceeds 5MB limit"
            });
        }

        try
        {
            _logger.LogInformation("Scanning image {FileName} ({Size} bytes)", file.FileName, file.Length);

            // Convert file to memory stream
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Create Rekognition request
            var detectModerationLabelsRequest = new DetectModerationLabelsRequest
            {
                Image = new Image
                {
                    Bytes = memoryStream
                },
                MinConfidence = 60F // Confidence threshold (60%)
            };

            // Call AWS Rekognition
            var response = await _rekognitionClient.DetectModerationLabelsAsync(detectModerationLabelsRequest);

            // Check for inappropriate content
            var detectedLabels = response.ModerationLabels
                .Select(label => $"{label.Name} ({label.Confidence:F2}%)")
                .ToList();

            var hasExplicitNudity = response.ModerationLabels
                .Any(label => label.Name.Contains("Explicit Nudity", StringComparison.OrdinalIgnoreCase));

            var hasViolence = response.ModerationLabels
                .Any(label => label.Name.Contains("Violence", StringComparison.OrdinalIgnoreCase) ||
                             label.Name.Contains("Graphic Violence", StringComparison.OrdinalIgnoreCase));

            if (hasExplicitNudity || hasViolence)
            {
                _logger.LogWarning(
                    "Image {FileName} rejected. Detected labels: {Labels}",
                    file.FileName,
                    string.Join(", ", detectedLabels));

                return BadRequest(new ScanImageResponse
                {
                    IsApproved = false,
                    Message = "Image contains inappropriate content and cannot be uploaded.",
                    DetectedLabels = detectedLabels
                });
            }

            // Image is safe
            _logger.LogInformation(
                "Image {FileName} approved. Labels detected: {Labels}",
                file.FileName,
                detectedLabels.Any() ? string.Join(", ", detectedLabels) : "None");

            return Ok(new ScanImageResponse
            {
                IsApproved = true,
                Message = "Image is safe to upload.",
                DetectedLabels = detectedLabels.Any() ? detectedLabels : null
            });
        }
        catch (AmazonRekognitionException ex)
        {
            _logger.LogError(ex, "AWS Rekognition error scanning image {FileName}", file.FileName);
            return StatusCode(500, new ScanImageResponse
            {
                IsApproved = false,
                Message = $"Error scanning image: {ex.Message}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error scanning image {FileName}", file.FileName);
            return StatusCode(500, new ScanImageResponse
            {
                IsApproved = false,
                Message = "An unexpected error occurred while scanning the image."
            });
        }
    }
}

