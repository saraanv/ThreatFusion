using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreatFusion.Threat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThreatFeedSyncCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SkippedCount",
                table: "ThreatFeedSyncs",
                newName: "UpdatedCount");

            migrationBuilder.RenameColumn(
                name: "ImportedCount",
                table: "ThreatFeedSyncs",
                newName: "UnchangedCount");

            migrationBuilder.AddColumn<int>(
                name: "CreatedCount",
                table: "ThreatFeedSyncs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedCount",
                table: "ThreatFeedSyncs");

            migrationBuilder.RenameColumn(
                name: "UpdatedCount",
                table: "ThreatFeedSyncs",
                newName: "SkippedCount");

            migrationBuilder.RenameColumn(
                name: "UnchangedCount",
                table: "ThreatFeedSyncs",
                newName: "ImportedCount");
        }
    }
}
