using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Escort.User.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixuserdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "UserContactDetails");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "UserContactDetails");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserVerificationDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserContactDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserVerificationDetails");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserContactDetails");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "UserContactDetails",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "UserContactDetails",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
