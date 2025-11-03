using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBB_ApplicationDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedAtInAccreditation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Accreditations",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Accreditations");
        }
    }
}
