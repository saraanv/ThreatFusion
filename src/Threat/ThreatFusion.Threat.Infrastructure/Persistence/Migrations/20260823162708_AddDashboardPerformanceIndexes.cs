using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreatFusion.Threat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ThreatWatchlists_UserId_IsActive_IsDeleted",
                table: "ThreatWatchlists",
                columns: new[] { "UserId", "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ThreatAlerts_UserId_CreatedAtUtc",
                table: "ThreatAlerts",
                columns: new[] { "UserId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ThreatAlerts_UserId_IsRead_IsDeleted",
                table: "ThreatAlerts",
                columns: new[] { "UserId", "IsRead", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ThreatWatchlists_UserId_IsActive_IsDeleted",
                table: "ThreatWatchlists");

            migrationBuilder.DropIndex(
                name: "IX_ThreatAlerts_UserId_CreatedAtUtc",
                table: "ThreatAlerts");

            migrationBuilder.DropIndex(
                name: "IX_ThreatAlerts_UserId_IsRead_IsDeleted",
                table: "ThreatAlerts");
        }
    }
}
