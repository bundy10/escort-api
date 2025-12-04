using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Escort.Listing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixListingdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListingDetails_Listings_Id",
                table: "ListingDetails");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ListingDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "ListingId",
                table: "ListingDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ListingDetails_ListingId",
                table: "ListingDetails",
                column: "ListingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ListingDetails_Listings_ListingId",
                table: "ListingDetails",
                column: "ListingId",
                principalTable: "Listings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListingDetails_Listings_ListingId",
                table: "ListingDetails");

            migrationBuilder.DropIndex(
                name: "IX_ListingDetails_ListingId",
                table: "ListingDetails");

            migrationBuilder.DropColumn(
                name: "ListingId",
                table: "ListingDetails");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "ListingDetails",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_ListingDetails_Listings_Id",
                table: "ListingDetails",
                column: "Id",
                principalTable: "Listings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
