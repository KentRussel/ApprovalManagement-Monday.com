using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApprovalManagement.Migrations
{
    public partial class addApprovalReportPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalReport",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalReportPath",
                table: "Projects",
                type: "nvarchar(50)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalReportPath",
                table: "Projects");

            migrationBuilder.AddColumn<byte[]>(
                name: "ApprovalReport",
                table: "Projects",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
