using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Escort.Event.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removingboolcompletedtostatusstring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Events",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Events");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
