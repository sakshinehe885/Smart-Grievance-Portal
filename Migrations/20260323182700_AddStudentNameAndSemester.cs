using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartGrievancePortal.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentNameAndSemester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentSemester",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentName",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "StudentSemester",
                table: "Complaints");
        }
    }
}
