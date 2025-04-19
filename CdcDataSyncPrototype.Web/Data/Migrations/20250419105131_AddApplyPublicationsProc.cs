using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CdcDataSyncPrototype.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddApplyPublicationsProc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(File.ReadAllText("Data/Scripts/sp_ApplyPublicationsFromStaging.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.sp_ApplyPublicationsFromStaging;");
        }
    }
}
