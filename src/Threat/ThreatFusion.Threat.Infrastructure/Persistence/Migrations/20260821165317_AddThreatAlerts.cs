using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreatFusion.Threat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThreatAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThreatAlerts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ThreatIndicatorId = table.Column<long>(type: "bigint", nullable: false),
                    AlertType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreatAlerts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThreatAlerts_ThreatIndicatorId",
                table: "ThreatAlerts",
                column: "ThreatIndicatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatAlerts_UserId_IsRead",
                table: "ThreatAlerts",
                columns: new[] { "UserId", "IsRead" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThreatAlerts");
        }
    }
}
