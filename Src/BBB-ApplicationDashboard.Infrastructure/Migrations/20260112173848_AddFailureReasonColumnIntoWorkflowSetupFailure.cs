using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BBB_ApplicationDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFailureReasonColumnIntoWorkflowSetupFailure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                table: "WorkflowSetupFailures",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailureReason",
                table: "WorkflowSetupFailures");
        }
    }
}
