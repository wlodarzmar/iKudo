using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace iKudo.Domain.Migrations
{
    public partial class KudoAddedToNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KudoId",
                table: "Notifications",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_KudoId",
                table: "Notifications",
                column: "KudoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Kudos_KudoId",
                table: "Notifications",
                column: "KudoId",
                principalTable: "Kudos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Kudos_KudoId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_KudoId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "KudoId",
                table: "Notifications");
        }
    }
}
