using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBB_ApplicationDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeResolvedByColumnFromUserToEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowSetupFailures_Users_ResolvedByUserId",
                table: "WorkflowSetupFailures"
            );

            migrationBuilder.DropIndex(
                name: "IX_WorkflowSetupFailures_ResolvedByUserId",
                table: "WorkflowSetupFailures"
            );

            migrationBuilder.DropColumn(name: "ResolvedByUserId", table: "WorkflowSetupFailures");

            migrationBuilder.AddColumn<string>(
                name: "ResolvedBy",
                table: "WorkflowSetupFailures",
                type: "text",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ResolvedBy", table: "WorkflowSetupFailures");

            migrationBuilder.AddColumn<Guid>(
                name: "ResolvedByUserId",
                table: "WorkflowSetupFailures",
                type: "uuid",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSetupFailures_ResolvedByUserId",
                table: "WorkflowSetupFailures",
                column: "ResolvedByUserId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowSetupFailures_Users_ResolvedByUserId",
                table: "WorkflowSetupFailures",
                column: "ResolvedByUserId",
                principalTable: "Users",
                principalColumn: "UserId"
            );
        }
    }
}
