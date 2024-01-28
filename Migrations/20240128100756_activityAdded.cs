using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeManagement.Migrations
{
    public partial class activityAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "activity",
                columns: table => new
                {
                    ActivityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsersAssigned = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity", x => x.ActivityId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity");
        }
    }
}
