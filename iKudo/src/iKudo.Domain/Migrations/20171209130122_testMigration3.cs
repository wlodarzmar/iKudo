using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace iKudo.Domain.Migrations
{
    public partial class testMigration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Test",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "Test2",
                table: "Boards");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Test",
                table: "Boards",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Test2",
                table: "Boards",
                nullable: true);
        }
    }
}
