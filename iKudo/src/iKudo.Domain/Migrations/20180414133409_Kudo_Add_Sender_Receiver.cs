using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace iKudo.Domain.Migrations
{
    public partial class Kudo_Add_Sender_Receiver : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "Kudos",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverId",
                table: "Kudos",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Kudos_ReceiverId",
                table: "Kudos",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Kudos_SenderId",
                table: "Kudos",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kudos_Users_ReceiverId",
                table: "Kudos",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Kudos_Users_SenderId",
                table: "Kudos",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kudos_Users_ReceiverId",
                table: "Kudos");

            migrationBuilder.DropForeignKey(
                name: "FK_Kudos_Users_SenderId",
                table: "Kudos");

            migrationBuilder.DropIndex(
                name: "IX_Kudos_ReceiverId",
                table: "Kudos");

            migrationBuilder.DropIndex(
                name: "IX_Kudos_SenderId",
                table: "Kudos");

            migrationBuilder.AlterColumn<string>(
                name: "SenderId",
                table: "Kudos",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverId",
                table: "Kudos",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
