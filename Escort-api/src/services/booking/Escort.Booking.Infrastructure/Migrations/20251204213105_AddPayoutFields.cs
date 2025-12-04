using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Escort.Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayoutFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PayoutDueAt",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PayoutProcessed",
                table: "Bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayoutDueAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PayoutProcessed",
                table: "Bookings");
        }
    }
}
