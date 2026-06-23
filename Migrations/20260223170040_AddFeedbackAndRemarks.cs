using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartGrievancePortal.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedbackAndRemarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "Complaints",
                newName: "StudentFeedback");

            migrationBuilder.AddColumn<string>(
                name: "AdminResponse",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Complaints",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaffRemarks",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminResponse",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "StaffRemarks",
                table: "Complaints");

            migrationBuilder.RenameColumn(
                name: "StudentFeedback",
                table: "Complaints",
                newName: "Remarks");
        }
    }
}
