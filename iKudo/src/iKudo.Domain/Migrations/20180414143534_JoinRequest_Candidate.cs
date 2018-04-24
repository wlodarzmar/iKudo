using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace iKudo.Domain.Migrations
{
    public partial class JoinRequest_Candidate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CandidateId",
                table: "JoinRequests",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_JoinRequests_CandidateId",
                table: "JoinRequests",
                column: "CandidateId");

            migrationBuilder.AddForeignKey(
                name: "FK_JoinRequests_Users_CandidateId",
                table: "JoinRequests",
                column: "CandidateId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JoinRequests_Users_CandidateId",
                table: "JoinRequests");

            migrationBuilder.DropIndex(
                name: "IX_JoinRequests_CandidateId",
                table: "JoinRequests");

            migrationBuilder.AlterColumn<string>(
                name: "CandidateId",
                table: "JoinRequests",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
