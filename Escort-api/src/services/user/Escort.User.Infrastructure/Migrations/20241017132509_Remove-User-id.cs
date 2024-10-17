using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Escort.User.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserVerificationDetails");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserContactDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserVerificationDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserContactDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
