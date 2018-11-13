using iKudo.Domain.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;

namespace iKudo.Domain.Migrations
{
    public partial class BoardArchiveTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = this.ReadSqlScript("iKudo.Domain.Migrations.20181112160330_Add_ArchiveBoardTable_Add_BoardTriggers.sql");
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sql = this.ReadSqlScript("iKudo.Domain.Migrations.20181112160330_Revert.sql");
            migrationBuilder.Sql(sql);
        }
    }
}
