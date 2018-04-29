using iKudo.Domain.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace iKudo.Domain.Migrations
{
    public partial class KudoAcceptance_KudoStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Kudos",
                nullable: false,
                defaultValue: (int)KudoStatus.Accepted); //NOTE: all existing kudos accepted

            migrationBuilder.AddColumn<bool>(
                name: "KudoAcceptanceEnabled",
                table: "Boards",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "KudoAcceptanceFromExternalUsersEnabled",
                table: "Boards",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Kudos");

            migrationBuilder.DropColumn(
                name: "KudoAcceptanceEnabled",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "KudoAcceptanceFromExternalUsersEnabled",
                table: "Boards");
        }
    }
}
