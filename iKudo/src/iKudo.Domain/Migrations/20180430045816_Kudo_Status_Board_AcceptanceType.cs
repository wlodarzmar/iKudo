using iKudo.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace iKudo.Domain.Migrations
{
    public partial class Kudo_Status_Board_AcceptanceType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Kudos",
                nullable: false,
                defaultValue: (int)KudoStatus.Accepted);

            migrationBuilder.AddColumn<int>(
                name: "AcceptanceType",
                table: "Boards",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Kudos");

            migrationBuilder.DropColumn(
                name: "AcceptanceType",
                table: "Boards");
        }
    }
}
