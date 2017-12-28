using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace iKudo.Domain.Migrations
{
    public partial class kudoImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Kudos",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Kudos");
        }
    }
}
