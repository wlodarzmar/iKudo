using Microsoft.EntityFrameworkCore.Migrations;

namespace iKudo.Domain.Migrations
{
    public partial class Users_FirstName_Nullable_Email_Or_FirstName_Required : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                nullable: true,
                oldClrType: typeof(string));

            string sql = @"ALTER TABLE dbo.Users ADD CONSTRAINT CK_FirstName_Or_Email CHECK (FirstName IS NOT NULL OR Email IS NOT NULL);";
            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            string sql = @"ALTER TABLE dbo.Users DROP CONSTRAINT CK_FirstName_Or_Email";
            migrationBuilder.Sql(sql);
        }
    }
}
