# Content Moderation API - AWS Rekognition Integration

## Overview
The ContentController provides image content moderation using AWS Rekognition to scan images for inappropriate content before allowing upload to S3.

## Features
- **Automated Content Moderation**: Uses AWS Rekognition's DetectModerationLabels API
- **Explicit Content Detection**: Blocks images containing explicit nudity
- **Violence Detection**: Blocks images containing violence or graphic violence
- **File Validation**: Validates file type, size, and format
- **Detailed Logging**: Comprehensive logs for monitoring and auditing
- **Frontend Integration**: Returns clear approval/rejection status

## Endpoint

### POST /api/content/scan-image

Scans an uploaded image for inappropriate content before S3 upload.

**Request:**
- Method: `POST`
- Content-Type: `multipart/form-data`
- Body: Form data with file field

**Parameters:**
- `file` (IFormFile): The image file to scan
  - Supported formats: JPEG, PNG
  - Maximum size: 5MB
  - Minimum confidence threshold: 60%

**Success Response (200 OK):**
```json
{
  "isApproved": true,
  "message": "Image is safe to upload.",
  "detectedLabels": null
}
```

**Rejection Response (400 Bad Request):**
```json
{
  "isApproved": false,
  "message": "Image contains inappropriate content and cannot be uploaded.",
  "detectedLabels": [
    "Explicit Nudity (95.67%)",
    "Revealing Clothes (87.23%)"
  ]
}
```

**Error Response (500 Internal Server Error):**
```json
{
  "isApproved": false,
  "message": "Error scanning image: [error details]"
}
```

## AWS Configuration

### Environment Variables (.env file)
```bash
# AWS Configuration for Rekognition
AWS_ACCESS_KEY_ID=your_access_key_here
AWS_SECRET_ACCESS_KEY=your_secret_key_here
AWS_REGION=us-east-1
```

### appsettings.json (Alternative)
```json
{
  "AWS": {
    "AccessKeyId": "your_access_key_here",
    "SecretAccessKey": "your_secret_key_here",
    "Region": "us-east-1"
  }
}
```

### IAM Permissions Required
The AWS credentials need the following IAM policy:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "rekognition:DetectModerationLabels"
      ],
      "Resource": "*"
    }
  ]
}
```

## Frontend Integration

### JavaScript/TypeScript Example

```typescript
async function scanImageBeforeUpload(file: File): Promise<boolean> {
  const formData = new FormData();
  formData.append('file', file);

  try {
    const response = await fetch('http://localhost:8086/api/content/scan-image', {
      method: 'POST',
      body: formData
    });

    const result = await response.json();

    if (response.ok && result.isApproved) {
      console.log('Image approved:', result.message);
      // Proceed with S3 upload
      return true;
    } else {
      console.error('Image rejected:', result.message);
      if (result.detectedLabels) {
        console.warn('Detected labels:', result.detectedLabels);
      }
      // Show error to user
      return false;
    }
  } catch (error) {
    console.error('Error scanning image:', error);
    return false;
  }
}

// Usage in file upload handler
async function handleFileUpload(event: Event) {
  const input = event.target as HTMLInputElement;
  const file = input.files?.[0];
  
  if (!file) return;

  // First, scan the image
  const isApproved = await scanImageBeforeUpload(file);
  
  if (isApproved) {
    // Image is safe, proceed with S3 upload
    await uploadToS3(file);
    console.log('Image uploaded successfully');
  } else {
    // Image rejected, show error to user
    alert('This image contains inappropriate content and cannot be uploaded.');
  }
}
```

### React Hook Example

```typescript
import { useState } from 'react';

interface ScanResult {
  isApproved: boolean;
  message: string;
  detectedLabels?: string[];
}

export const useImageScanner = () => {
  const [isScanning, setIsScanning] = useState(false);
  const [scanResult, setScanResult] = useState<ScanResult | null>(null);

  const scanImage = async (file: File): Promise<boolean> => {
    setIsScanning(true);
    setScanResult(null);

    const formData = new FormData();
    formData.append('file', file);

    try {
      const response = await fetch('http://localhost:8086/api/content/scan-image', {
        method: 'POST',
        body: formData
      });

      const result: ScanResult = await response.json();
      setScanResult(result);

      return response.ok && result.isApproved;
    } catch (error) {
      setScanResult({
        isApproved: false,
        message: 'Error scanning image'
      });
      return false;
    } finally {
      setIsScanning(false);
    }
  };

  return { scanImage, isScanning, scanResult };
};

// Usage in component
function ImageUploadForm() {
  const { scanImage, isScanning, scanResult } = useImageScanner();

  const handleUpload = async (file: File) => {
    const isApproved = await scanImage(file);
    
    if (isApproved) {
      // Proceed with S3 upload
      await uploadToS3(file);
    }
  };

  return (
    <div>
      <input type="file" onChange={(e) => handleUpload(e.target.files[0])} />
      {isScanning && <p>Scanning image...</p>}
      {scanResult && !scanResult.isApproved && (
        <p className="error">{scanResult.message}</p>
      )}
    </div>
  );
}
```

### cURL Example

```bash
curl -X POST http://localhost:8086/api/content/scan-image \
  -F "file=@/path/to/image.jpg" \
  -H "Content-Type: multipart/form-data"
```

## Moderation Labels Detected

AWS Rekognition can detect various types of inappropriate content:

### Explicit Content
- Explicit Nudity
- Nudity
- Sexual Activity
- Partial Nudity
- Revealing Clothes

### Violence
- Violence
- Graphic Violence
- Physical Violence
- Weapon Violence

### Other Categories
- Visually Disturbing
- Rude Gestures
- Drugs
- Tobacco
- Alcohol
- Gambling
- Hate Symbols

**Note:** The current implementation only blocks:
- Explicit Nudity
- Violence (including Graphic Violence)

Other labels are detected and logged but don't block the upload.

## Confidence Threshold

The service uses a **60% confidence threshold** for moderation labels. This means:
- Labels with confidence ≥ 60% are reported
- You can adjust this in `ContentController.cs` by changing `MinConfidence`

```csharp
MinConfidence = 60F // Adjust between 0-100
```

**Recommended thresholds:**
- **80-90%**: More permissive (fewer false positives)
- **60-70%**: Balanced (default)
- **40-50%**: More restrictive (more false positives)

## Validation Rules

### File Type
- **Allowed**: JPEG, JPG, PNG
- **Blocked**: GIF, BMP, WEBP, etc.

### File Size
- **Maximum**: 5MB
- **Recommendation**: Consider reducing to 2-3MB for faster processing

### Image Requirements
- Must be a valid image file
- No minimum dimensions (handled by Rekognition)

## Error Handling

### Common Errors

#### 1. No AWS Credentials
```
AmazonServiceException: No credentials specified
```
**Solution**: Configure AWS credentials in .env file or IAM role

#### 2. Invalid Region
```
AmazonServiceException: Invalid region
```
**Solution**: Use a valid AWS region (us-east-1, us-west-2, etc.)

#### 3. Insufficient Permissions
```
AccessDeniedException: User is not authorized to perform: rekognition:DetectModerationLabels
```
**Solution**: Add DetectModerationLabels permission to IAM user/role

#### 4. Image Too Large
```
BadRequest: File size exceeds 5MB limit
```
**Solution**: Resize image on frontend before uploading

#### 5. Invalid Image Format
```
BadRequest: Invalid file type
```
**Solution**: Ensure file is JPEG or PNG format

## Logging

All moderation requests are logged with:

### Approved Images
```
info: Escort.Safety.API.Controllers.ContentController[0]
      Scanning image profile.jpg (1234567 bytes)
info: Escort.Safety.API.Controllers.ContentController[0]
      Image profile.jpg approved. Labels detected: None
```

### Rejected Images
```
info: Escort.Safety.API.Controllers.ContentController[0]
      Scanning image inappropriate.jpg (2345678 bytes)
warn: Escort.Safety.API.Controllers.ContentController[0]
      Image inappropriate.jpg rejected. Detected labels: Explicit Nudity (95.67%), Nudity (87.23%)
```

### Errors
```
error: Escort.Safety.API.Controllers.ContentController[0]
      AWS Rekognition error scanning image test.jpg
      Amazon.Rekognition.AmazonRekognitionException: InvalidImageFormatException
```

## Performance Considerations

### Response Times
- **Average**: 500-1500ms per image
- **Factors**: Image size, AWS region latency, network speed

### Cost
AWS Rekognition pricing (as of 2025):
- **First 1M images/month**: $1.00 per 1,000 images
- **Next 9M images/month**: $0.80 per 1,000 images
- **Over 10M images/month**: $0.60 per 1,000 images

**Example costs:**
- 1,000 scans/month: ~$1.00
- 10,000 scans/month: ~$10.00
- 100,000 scans/month: ~$90.00

### Optimization Tips
1. **Client-side validation**: Check file type/size before uploading
2. **Image compression**: Resize large images on frontend
3. **Caching**: Cache scan results for identical images (by hash)
4. **Rate limiting**: Prevent abuse with rate limits
5. **Batch processing**: For bulk uploads, consider async processing

## Testing

### Unit Testing
```csharp
[Fact]
public async Task ScanImage_RejectsExplicitContent()
{
    // Arrange
    var controller = new ContentController(logger, rekognitionClient);
    var file = CreateMockFile("explicit.jpg");

    // Act
    var result = await controller.ScanImage(file);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(result);
    var response = Assert.IsType<ScanImageResponse>(badRequest.Value);
    Assert.False(response.IsApproved);
}
```

### Integration Testing
```bash
# Test with a safe image
curl -X POST http://localhost:8086/api/content/scan-image \
  -F "file=@test-safe.jpg"

# Test with invalid file type
curl -X POST http://localhost:8086/api/content/scan-image \
  -F "file=@test.gif"

# Test with no file
curl -X POST http://localhost:8086/api/content/scan-image
```

## Security Best Practices

### 1. API Authentication
Add authentication to prevent abuse:
```csharp
[Authorize]
[HttpPost("scan-image")]
public async Task<IActionResult> ScanImage(IFormFile? file)
```

### 2. Rate Limiting
Implement rate limiting per user/IP:
```csharp
[RateLimit(MaxRequests = 10, TimeWindow = 60)] // 10 requests per minute
```

### 3. File Validation
Already implemented:
- File type validation
- File size limits
- Content validation via Rekognition

### 4. Logging & Monitoring
- Log all moderation attempts
- Monitor rejection rates
- Alert on unusual patterns

### 5. Content Audit Trail
Consider storing scan results for compliance:
```csharp
await _auditRepository.LogModerationResult(userId, fileName, isApproved, detectedLabels);
```

## Deployment Checklist

- [ ] Configure AWS credentials (environment variables or IAM role)
- [ ] Set appropriate AWS region
- [ ] Verify IAM permissions for DetectModerationLabels
- [ ] Test with sample images (safe and inappropriate)
- [ ] Configure rate limiting
- [ ] Set up monitoring and alerting
- [ ] Document frontend integration requirements
- [ ] Test error scenarios
- [ ] Review and adjust confidence threshold
- [ ] Implement authentication if needed

## Monitoring & Alerts

### Key Metrics
- Total scans per day/hour
- Rejection rate (%)
- Average response time
- Error rate
- Cost per scan

### Recommended Alerts
- Rejection rate > 50% (possible attack or misconfiguration)
- Error rate > 5%
- Response time > 3 seconds
- Daily cost > threshold

## Future Enhancements

### Potential Features
- [ ] Support for video content moderation
- [ ] Custom moderation labels
- [ ] Face detection and comparison
- [ ] Text detection in images (OCR)
- [ ] Celebrity recognition
- [ ] Multiple image batch processing
- [ ] Webhook notifications for async processing
- [ ] Admin dashboard for reviewing flagged content
- [ ] Machine learning model training on feedback

## Dependencies
- AWSSDK.Rekognition (4.0.3.5)
- AWSSDK.Core (4.0.3.3)
- ASP.NET Core 8.0

## Support

For issues or questions:
- Check AWS Rekognition status: https://status.aws.amazon.com/
- Review AWS Rekognition documentation
- Check application logs for detailed error messages
- Verify AWS credentials and permissions

