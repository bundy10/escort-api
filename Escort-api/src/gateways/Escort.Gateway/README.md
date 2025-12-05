# Escort API Gateway - Ocelot Configuration

## Overview
The Escort.Gateway is the single entry point for all microservices in the Escort API ecosystem. It uses Ocelot, a .NET API Gateway, to route requests to the appropriate backend services.

## Architecture

### Gateway Benefits
- **Single Entry Point**: Frontend only needs to know one URL
- **Service Discovery**: Routes requests to appropriate microservices
- **Load Balancing**: Can distribute traffic across multiple instances
- **Request/Response Transformation**: Can modify headers, query strings, etc.
- **Authentication/Authorization**: Centralized security (future enhancement)
- **Rate Limiting**: Control traffic to backend services (future enhancement)
- **Logging & Monitoring**: Centralized request tracking

## Port Configuration

### Gateway Port
- **Local Development**: `http://localhost:8000`
- **Docker**: Port 8080 internally, mapped to 8000 externally

### Backend Services (Internal Docker Network)
All backend services run on port 8080 internally within Docker:
- User API: `escort.user.api:8080`
- Client API: `escort.client.api:8080`
- Driver API: `escort.driver.api:8080`
- Listing API: `escort.listing.api:8080`
- Booking API: `escort.booking.api:8080`
- Payment API: `escort.payment.api:8080`
- Safety API: `escort.safety.api:8080`
- Chat API: `escort.chat.api:8080`

## Route Configuration

### HTTP Routes

#### 1. User Service
- **Public URL**: `http://localhost:8000/api/users/{everything}`
- **Routes to**: `escort.user.api/api/users/{everything}`
- **Methods**: GET, POST, PUT, DELETE, PATCH

**Examples:**
```bash
# Get all users
GET http://localhost:8000/api/users

# Get user by ID
GET http://localhost:8000/api/users/123

# Create user
POST http://localhost:8000/api/users
```

#### 2. Client Service
- **Public URL**: `http://localhost:8000/api/clients/{everything}`
- **Routes to**: `escort.client.api/api/clients/{everything}`
- **Methods**: GET, POST, PUT, DELETE, PATCH

**Examples:**
```bash
# Get all clients
GET http://localhost:8000/api/clients

# Update client
PUT http://localhost:8000/api/clients/456
```

#### 3. Driver Service
- **Public URL**: `http://localhost:8000/api/driver/{everything}`
- **Routes to**: `escort.driver.api/api/driver/{everything}`
- **Methods**: GET, POST, PUT, DELETE, PATCH

**Note**: Public path is `/api/driver` (singular) for consistency

**Examples:**
```bash
# Get all drivers
GET http://localhost:8000/api/driver

# Create driver
POST http://localhost:8000/api/driver
```

#### 4. Listing Service
- **Public URL**: `http://localhost:8000/api/listings/{everything}`
- **Routes to**: `escort.listing.api/api/listings/{everything}`
- **Methods**: GET, POST, PUT, DELETE, PATCH

**Examples:**
```bash
# Search listings
GET http://localhost:8000/api/listings?location=NYC

# Get listing details
GET http://localhost:8000/api/listings/789
```

#### 5. Booking Service
- **Public URL**: `http://localhost:8000/api/bookings/{everything}`
- **Routes to**: `escort.booking.api/api/bookings/{everything}`
- **Methods**: GET, POST, PUT, DELETE, PATCH

**Examples:**
```bash
# Create booking
POST http://localhost:8000/api/bookings

# Confirm booking
PUT http://localhost:8000/api/bookings/123/confirm

# Complete booking
PUT http://localhost:8000/api/bookings/123/complete
```

#### 6. Payment Service
- **Public URL**: `http://localhost:8000/api/payments/{everything}`
- **Routes to**: `escort.payment.api/api/payments/{everything}`
- **Methods**: GET, POST, PUT, DELETE, PATCH

**Examples:**
```bash
# Create Stripe account
POST http://localhost:8000/api/payments/create-account

# Authorize payment
POST http://localhost:8000/api/payments/authorize-payment

# Capture payment
POST http://localhost:8000/api/payments/capture
```

#### 7. Safety Service
- **Public URL**: `http://localhost:8000/api/safety/{everything}`
- **Routes to**: `escort.safety.api/api/safety/{everything}`
- **Methods**: GET, POST, PUT, DELETE, PATCH

**Examples:**
```bash
# Verify identity
POST http://localhost:8000/api/safety/verify-identity

# Scan image content
POST http://localhost:8000/api/safety/scan-image
```

### WebSocket Route

#### 8. Chat Service (SignalR)
- **Public URL**: `ws://localhost:8000/chatHub`
- **Routes to**: `escort.chat.api/chatHub`
- **Protocol**: HTTP with WebSocket upgrade
- **Methods**: GET, POST

**SignalR Connection:**
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:8000/chatHub")
    .withAutomaticReconnect()
    .build();

await connection.start();
```

**Note**: Ocelot handles WebSocket upgrade automatically with proper header transformation.

## Running the Gateway

### Local Development

```bash
cd src/gateways/Escort.Gateway
dotnet run
```

The gateway will be available at `http://localhost:8000`

### Using Docker Compose

```bash
# Start all services including the gateway
docker-compose up

# Or start only the gateway
docker-compose up escort.gateway
```

### Testing the Gateway

```bash
# Test if gateway is routing correctly
curl http://localhost:8000/api/users
curl http://localhost:8000/api/listings
curl http://localhost:8000/api/bookings
```

## Configuration Files

### ocelot.json
Located at: `src/gateways/Escort.Gateway/ocelot.json`

This file defines all routing rules. Each route contains:
- **UpstreamPathTemplate**: The public URL pattern
- **UpstreamHttpMethod**: Allowed HTTP methods
- **DownstreamPathTemplate**: The internal service path
- **DownstreamScheme**: Protocol (http/https)
- **DownstreamHostAndPorts**: Service hostname and port

**Modifying Routes:**
1. Edit `ocelot.json`
2. Restart the Gateway service
3. Changes take effect immediately

### Example Route Configuration:
```json
{
  "UpstreamPathTemplate": "/api/bookings/{everything}",
  "UpstreamHttpMethod": [ "Get", "Post", "Put", "Delete" ],
  "DownstreamPathTemplate": "/api/bookings/{everything}",
  "DownstreamScheme": "http",
  "DownstreamHostAndPorts": [
    {
      "Host": "escort.booking.api",
      "Port": 8080
    }
  ]
}
```

## Frontend Integration

### Before Gateway (Direct Service Calls)
```javascript
// Had to know all service URLs
const userApi = 'http://localhost:8080';
const bookingApi = 'http://localhost:8088';
const paymentApi = 'http://localhost:8085';

// Different endpoints for different services
fetch(`${userApi}/api/users/123`);
fetch(`${bookingApi}/api/bookings/456`);
```

### After Gateway (Single Entry Point)
```javascript
// Single gateway URL
const apiGateway = 'http://localhost:8000';

// All requests go through gateway
fetch(`${apiGateway}/api/users/123`);
fetch(`${apiGateway}/api/bookings/456`);
fetch(`${apiGateway}/api/payments/create-account`);
```

### React Example
```typescript
// api.config.ts
export const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:8000';

// services/userService.ts
import { API_BASE_URL } from '../config/api.config';

export const getUser = async (id: number) => {
  const response = await fetch(`${API_BASE_URL}/api/users/${id}`);
  return response.json();
};

export const getBookings = async () => {
  const response = await fetch(`${API_BASE_URL}/api/bookings`);
  return response.json();
};
```

## Request Flow

### Example: Creating a Booking

```
1. Frontend makes request:
   POST http://localhost:8000/api/bookings
   Body: { date: "2025-12-10", ... }
   
   ↓
   
2. Gateway receives request
   - Matches route: /api/bookings/{everything}
   - Determines downstream: escort.booking.api:8080
   
   ↓
   
3. Gateway forwards to Booking API:
   POST http://escort.booking.api:8080/api/bookings
   Body: { date: "2025-12-10", ... }
   
   ↓
   
4. Booking API processes request
   - Creates booking
   - Returns response
   
   ↓
   
5. Gateway forwards response to frontend
   Status: 201 Created
   Body: { id: 123, status: "Pending", ... }
```

## CORS Configuration

The gateway is configured with an "AllowAll" CORS policy for development:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});
```

**Production Recommendation**: Restrict CORS to specific origins:
```csharp
policy => policy.WithOrigins("https://yourdomain.com")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials();
```

## Health Checks

### Checking Gateway Status
```bash
# Gateway should return 404 for root (expected)
curl http://localhost:8000/

# Test a real endpoint
curl http://localhost:8000/api/users
```

### Checking Backend Service Connectivity
If a route returns 503 Service Unavailable:
- Backend service is not running
- Backend service hostname is incorrect
- Backend service port is incorrect

## Troubleshooting

### Error: Cannot find service
**Problem**: `503 Service Unavailable`

**Solutions:**
1. Check if backend service is running: `docker ps`
2. Verify service name in ocelot.json matches docker-compose.yml
3. Check backend service port (should be 8080)

### Error: CORS policy blocks request
**Problem**: Browser blocks request due to CORS

**Solutions:**
1. Ensure CORS is enabled in Program.cs
2. Check that `app.UseCors("AllowAll")` is called
3. Verify frontend is making requests to gateway, not direct services

### Error: WebSocket connection failed
**Problem**: SignalR/Chat doesn't connect

**Solutions:**
1. Verify chat service is running
2. Check WebSocket headers in ocelot.json
3. Ensure frontend uses gateway URL for SignalR connection
4. Test with: `wscat -c ws://localhost:8000/chatHub`

### Error: Route not found (404)
**Problem**: Gateway returns 404 for valid path

**Solutions:**
1. Check route path in ocelot.json
2. Verify `{everything}` catchall is present
3. Ensure method is allowed (GET, POST, etc.)
4. Check for typos in path

## Performance Considerations

### Response Times
- **Gateway Overhead**: ~5-20ms per request
- **Total Time**: Gateway overhead + backend processing time

### Optimization Tips
1. **Connection Pooling**: Ocelot manages HTTP client connections efficiently
2. **No Rate Limiting**: Disabled by default for development
3. **Caching**: Can be added for GET requests (future enhancement)
4. **Load Balancing**: Can distribute across multiple backend instances

## Security Features

### Current Implementation
- ✅ CORS enabled for development
- ✅ HTTP routing
- ⬜ Authentication/Authorization (future)
- ⬜ Rate limiting (future)
- ⬜ Request throttling (future)

### Future Enhancements

#### 1. JWT Authentication
```json
{
  "AuthenticationOptions": {
    "AuthenticationProviderKey": "Bearer",
    "AllowedScopes": []
  }
}
```

#### 2. Rate Limiting
```json
{
  "RateLimitOptions": {
    "ClientWhitelist": [],
    "EnableRateLimiting": true,
    "Period": "1m",
    "Limit": 100
  }
}
```

#### 3. Request Aggregation
Combine multiple backend calls into single frontend request

#### 4. Response Caching
Cache GET requests to reduce backend load

## Monitoring & Logging

### Current Logging
- Ocelot logs all requests automatically
- Logs include: timestamp, path, status code, duration

### View Logs
```bash
# Docker logs
docker logs escort.gateway -f

# Local development
# Logs appear in console output
```

### Recommended Monitoring
- Add Application Insights for production
- Track response times per route
- Monitor error rates
- Alert on 503 (service unavailable) errors

## Deployment

### Development
```bash
dotnet run
```

### Docker
```bash
docker-compose up escort.gateway
```

### Production Considerations
1. **HTTPS**: Enable SSL/TLS
2. **Authentication**: Add JWT validation
3. **Rate Limiting**: Prevent abuse
4. **CORS**: Restrict to specific origins
5. **Health Checks**: Implement /health endpoint
6. **Logging**: Add structured logging
7. **Monitoring**: Add APM (Application Performance Monitoring)

## Environment Variables

The gateway can be configured via environment variables:

```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
```

## Dependencies

- **Ocelot**: 24.0.1 (API Gateway framework)
- **ASP.NET Core**: 9.0
- **Newtonsoft.Json**: 12.0.1 (JSON handling)
- **FluentValidation**: 11.11.0 (Request validation)

## Summary

The API Gateway provides:
- ✅ Single entry point for all services
- ✅ Automatic routing based on URL path
- ✅ WebSocket support for SignalR/Chat
- ✅ CORS configuration
- ✅ Easy to extend with new routes
- ✅ Docker support
- ✅ Production-ready architecture

All frontend applications should use `http://localhost:8000` as the base API URL instead of individual service URLs.

