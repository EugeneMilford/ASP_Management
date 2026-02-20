using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeManagement.Migrations
{
    public partial class DemoAdminFunctionsUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Summary",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemporary",
                table: "Summary",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TempUserId",
                table: "Summary",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "project",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemporary",
                table: "project",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TempUserId",
                table: "project",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "assignment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemporary",
                table: "assignment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TempUserId",
                table: "assignment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Activities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemporary",
                table: "Activities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TempUserId",
                table: "Activities",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Summary");

            migrationBuilder.DropColumn(
                name: "IsTemporary",
                table: "Summary");

            migrationBuilder.DropColumn(
                name: "TempUserId",
                table: "Summary");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "project");

            migrationBuilder.DropColumn(
                name: "IsTemporary",
                table: "project");

            migrationBuilder.DropColumn(
                name: "TempUserId",
                table: "project");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "assignment");

            migrationBuilder.DropColumn(
                name: "IsTemporary",
                table: "assignment");

            migrationBuilder.DropColumn(
                name: "TempUserId",
                table: "assignment");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "IsTemporary",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "TempUserId",
                table: "Activities");
        }
    }
}
