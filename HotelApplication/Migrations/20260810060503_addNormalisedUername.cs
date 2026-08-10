using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelApplication.Migrations
{
    /// <inheritdoc />
    public partial class addNormalisedUername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "normalised_username",
                table: "reservations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "normalised_username",
                table: "reservations");
        }
    }
}
