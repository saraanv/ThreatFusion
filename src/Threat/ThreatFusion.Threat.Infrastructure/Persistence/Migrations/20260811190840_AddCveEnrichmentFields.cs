using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreatFusion.Threat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCveEnrichmentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CvssScore",
                table: "ThreatIndicators",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CvssVector",
                table: "ThreatIndicators",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CvssVersion",
                table: "ThreatIndicators",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CweId",
                table: "ThreatIndicators",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceUrl",
                table: "ThreatIndicators",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CvssScore",
                table: "ThreatIndicators");

            migrationBuilder.DropColumn(
                name: "CvssVector",
                table: "ThreatIndicators");

            migrationBuilder.DropColumn(
                name: "CvssVersion",
                table: "ThreatIndicators");

            migrationBuilder.DropColumn(
                name: "CweId",
                table: "ThreatIndicators");

            migrationBuilder.DropColumn(
                name: "ReferenceUrl",
                table: "ThreatIndicators");
        }
    }
}
