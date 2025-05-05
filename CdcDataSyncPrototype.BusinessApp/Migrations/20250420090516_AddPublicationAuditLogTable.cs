using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CdcDataSyncPrototype.BusinessApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicationAuditLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishedDate",
                table: "Publications");

            migrationBuilder.AddColumn<bool>(
                name: "InternalOnly",
                table: "Publications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishEndDate",
                table: "Publications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishStartDate",
                table: "Publications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PublicationAuditLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicationId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RuleOutcome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicationAuditLog", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicationAuditLog");

            migrationBuilder.DropColumn(
                name: "InternalOnly",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "PublishEndDate",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "PublishStartDate",
                table: "Publications");

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedDate",
                table: "Publications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
