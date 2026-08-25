using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreatFusion.Threat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThreatRelationProvenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DiscoveredAtUtc",
                table: "ThreatIndicatorRelations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsAutomatic",
                table: "ThreatIndicatorRelations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SourceName",
                table: "ThreatIndicatorRelations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscoveredAtUtc",
                table: "ThreatIndicatorRelations");

            migrationBuilder.DropColumn(
                name: "IsAutomatic",
                table: "ThreatIndicatorRelations");

            migrationBuilder.DropColumn(
                name: "SourceName",
                table: "ThreatIndicatorRelations");
        }
    }
}
