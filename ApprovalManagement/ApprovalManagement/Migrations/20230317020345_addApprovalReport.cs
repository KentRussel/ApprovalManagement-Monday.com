using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApprovalManagement.Migrations
{
    public partial class addApprovalReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ApprovalReport",
                table: "Projects",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalReport",
                table: "Projects");
        }
    }
}
