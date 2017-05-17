using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace iKudo.Domain.Migrations
{
    public partial class userBoard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBoards",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    BoardId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBoards", x => new { x.UserId, x.BoardId });
                    table.ForeignKey(
                        name: "FK_UserBoards_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBoards_BoardId",
                table: "UserBoards",
                column: "BoardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBoards");
        }
    }
}
