using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.Migrations
{
    /// <inheritdoc />
    public partial class NotificationService_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "NotificationService");

            migrationBuilder.CreateTable(
                name: "users_notification_settings",
                schema: "NotificationService",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    send_to_email = table.Column<bool>(type: "boolean", nullable: false),
                    send_to_telegram = table.Column<bool>(type: "boolean", nullable: false),
                    send_to_website = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users_notification_settings", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users_notification_settings",
                schema: "NotificationService");
        }
    }
}
