#!/bin/bash

# Script to run migrations for all services

echo "========================================"
echo "Running Migrations for All Services"
echo "========================================"

# Define services with their infrastructure and startup projects
services=(
    "user:src/services/user/Escort.User.Infrastructure:src/services/user/Escort.User.API"
    "booking:src/services/booking/Escort.Booking.Infrastructure:src/services/booking/Escort.Booking.API"
    "client:src/services/client/Escort.Client.Infrastructure:src/services/client/Escort.Client.API"
    "driver:src/services/driver/Escort.Driver.Infrastructure:src/services/driver/Escort.Driver.API"
    "event:src/services/event/Escort.Event.Infrastructure:src/services/event/Escort.Event.API"
    "listing:src/services/listing/Escort.Listing.Infrastructure:src/services/listing/Escort.Listing.API"
)

# Migration name (can be passed as argument or default to "UpdatingBooking")
MIGRATION_NAME=${1:-UpdatingBooking}

echo ""
echo "Creating migrations with name: $MIGRATION_NAME"
echo "========================================"

# Create migrations for all services
for service in "${services[@]}"; do
    IFS=':' read -r name infra startup <<< "$service"
    echo ""
    echo "[$name] Creating migration..."
    dotnet ef migrations add "$MIGRATION_NAME" --project "$infra" --startup-project "$startup"
    
    if [ $? -eq 0 ]; then
        echo "[$name] ✅ Migration created successfully"
    else
        echo "[$name] ❌ Migration creation failed"
    fi
done

echo ""
echo "========================================"
echo "Applying migrations to database"
echo "========================================"

# Apply migrations to database
for service in "${services[@]}"; do
    IFS=':' read -r name infra startup <<< "$service"
    echo ""
    echo "[$name] Applying migration to database..."
    dotnet ef database update --project "$infra" --startup-project "$startup"
    
    if [ $? -eq 0 ]; then
        echo "[$name] ✅ Database updated successfully"
    else
        echo "[$name] ❌ Database update failed"
    fi
done

echo ""
echo "========================================"
echo "All migrations completed!"
echo "========================================"

