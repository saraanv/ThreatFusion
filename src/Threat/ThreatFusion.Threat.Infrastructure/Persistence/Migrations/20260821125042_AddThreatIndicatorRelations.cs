using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreatFusion.Threat.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThreatIndicatorRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThreatIndicatorRelations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceIndicatorId = table.Column<long>(type: "bigint", nullable: false),
                    TargetIndicatorId = table.Column<long>(type: "bigint", nullable: false),
                    RelationType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreatIndicatorRelations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ThreatIndicatorRelations_SourceIndicatorId_TargetIndicatorId_RelationType",
                table: "ThreatIndicatorRelations",
                columns: new[] { "SourceIndicatorId", "TargetIndicatorId", "RelationType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThreatIndicatorRelations");
        }
    }
}
