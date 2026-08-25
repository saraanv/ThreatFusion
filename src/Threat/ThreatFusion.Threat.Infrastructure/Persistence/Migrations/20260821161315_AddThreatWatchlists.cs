using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreatFusion.Threat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThreatWatchlists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThreatWatchlists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ThreatIndicatorId = table.Column<long>(type: "bigint", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreatWatchlists", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThreatWatchlists_UserId_ThreatIndicatorId",
                table: "ThreatWatchlists",
                columns: new[] { "UserId", "ThreatIndicatorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThreatWatchlists");
        }
    }
}
