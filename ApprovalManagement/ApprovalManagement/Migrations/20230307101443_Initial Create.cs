using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApprovalManagement.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedBudget = table.Column<int>(type: "int", nullable: false),
                    Manager = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ProjectStatus = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ApprovalRemarks = table.Column<string>(type: "nvarchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
