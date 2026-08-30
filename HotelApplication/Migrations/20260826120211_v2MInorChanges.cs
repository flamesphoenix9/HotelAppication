using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelApplication.Migrations
{
    /// <inheritdoc />
    public partial class v2MInorChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_room_type_prices_category_is_active",
                table: "room_type_prices");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "room_type_prices");

            migrationBuilder.CreateIndex(
                name: "ix_room_type_prices_category",
                table: "room_type_prices",
                column: "category",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_room_type_prices_category",
                table: "room_type_prices");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "room_type_prices",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_room_type_prices_category_is_active",
                table: "room_type_prices",
                columns: new[] { "category", "is_active" },
                unique: true);
        }
    }
}
