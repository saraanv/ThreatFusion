using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreatFusion.Threat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeDashboardQueries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ThreatAlerts_UserId_IsRead",
                table: "ThreatAlerts");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatIndicators_IsDeleted_IsActive_RiskLevel",
                table: "ThreatIndicators",
                columns: new[] { "IsDeleted", "IsActive", "RiskLevel" });

            migrationBuilder.CreateIndex(
                name: "IX_ThreatIndicators_IsDeleted_IsActive_RiskScore",
                table: "ThreatIndicators",
                columns: new[] { "IsDeleted", "IsActive", "RiskScore" });

            migrationBuilder.CreateIndex(
                name: "IX_ThreatIndicatorRelations_IsDeleted_IsActive_IsAutomatic",
                table: "ThreatIndicatorRelations",
                columns: new[] { "IsDeleted", "IsActive", "IsAutomatic" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ThreatIndicators_IsDeleted_IsActive_RiskLevel",
                table: "ThreatIndicators");

            migrationBuilder.DropIndex(
                name: "IX_ThreatIndicators_IsDeleted_IsActive_RiskScore",
                table: "ThreatIndicators");

            migrationBuilder.DropIndex(
                name: "IX_ThreatIndicatorRelations_IsDeleted_IsActive_IsAutomatic",
                table: "ThreatIndicatorRelations");

            migrationBuilder.CreateIndex(
                name: "IX_ThreatAlerts_UserId_IsRead",
                table: "ThreatAlerts",
                columns: new[] { "UserId", "IsRead" });
        }
    }
}
