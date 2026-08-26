using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelApplication.Migrations
{
    /// <inheritdoc />
    public partial class v2InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "room_type_prices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "text", nullable: false),
                    base_price_per_night = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_type_prices", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    room_number = table.Column<string>(type: "text", nullable: false),
                    room_category = table.Column<string>(type: "text", nullable: false),
                    price_per_night = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rooms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email_address = table.Column<string>(type: "text", nullable: false),
                    normalised_email = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    normalised_username = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reservation_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    reservation_group_code = table.Column<string>(type: "text", nullable: false),
                    amount_payable = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    reservation_group_status = table.Column<string>(type: "text", nullable: false),
                    payment_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation_groups", x => x.id);
                    table.ForeignKey(
                        name: "fk_reservation_groups_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    reservation_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expected_check_in = table.Column<DateOnly>(type: "date", nullable: false),
                    expected_check_out = table.Column<DateOnly>(type: "date", nullable: false),
                    actual_check_in_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actual_check_out_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    check_in_by_name = table.Column<string>(type: "text", nullable: true),
                    check_in_by_email_address = table.Column<string>(type: "text", nullable: true),
                    reservation_status = table.Column<string>(type: "text", nullable: false),
                    reservation_category = table.Column<string>(type: "text", nullable: false),
                    assigned_room_id = table.Column<Guid>(type: "uuid", nullable: true),
                    assigned_room_number = table.Column<string>(type: "text", nullable: true),
                    price_per_night = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservations", x => x.id);
                    table.ForeignKey(
                        name: "fk_reservations_reservation_groups_reservation_group_id",
                        column: x => x.reservation_group_id,
                        principalTable: "reservation_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_reservations_rooms_assigned_room_id",
                        column: x => x.assigned_room_id,
                        principalTable: "rooms",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "room_date_allocations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    room_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    reservation_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reservation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_room_date_allocations", x => x.id);
                    table.ForeignKey(
                        name: "fk_room_date_allocations_reservations_reservation_id",
                        column: x => x.reservation_id,
                        principalTable: "reservations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_date_allocations_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_reservation_groups_reservation_group_code",
                table: "reservation_groups",
                column: "reservation_group_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reservation_groups_user_id",
                table: "reservation_groups",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservations_assigned_room_id",
                table: "reservations",
                column: "assigned_room_id");

            migrationBuilder.CreateIndex(
                name: "ix_reservations_reservation_group_id",
                table: "reservations",
                column: "reservation_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_date_allocations_date_room_id",
                table: "room_date_allocations",
                columns: new[] { "date", "room_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_room_date_allocations_reservation_id",
                table: "room_date_allocations",
                column: "reservation_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_date_allocations_room_id",
                table: "room_date_allocations",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_type_prices_category_is_active",
                table: "room_type_prices",
                columns: new[] { "category", "is_active" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_rooms_room_number",
                table: "rooms",
                column: "room_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_email_address",
                table: "users",
                column: "email_address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_normalised_email",
                table: "users",
                column: "normalised_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_normalised_username",
                table: "users",
                column: "normalised_username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "room_date_allocations");

            migrationBuilder.DropTable(
                name: "room_type_prices");

            migrationBuilder.DropTable(
                name: "reservations");

            migrationBuilder.DropTable(
                name: "reservation_groups");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
