using Microsoft.EntityFrameworkCore.Migrations;

namespace iKudo.Domain.Migrations
{
    public partial class boardIsPrivate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Boards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            string sql = "UPDATE Boards SET IsPrivate = 1;";
            migrationBuilder.Sql(sql, true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Boards");
        }
    }
}
