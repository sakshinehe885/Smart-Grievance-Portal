using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartGrievancePortal.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStudentFieldsVertical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentName",
                table: "Complaints",
                newName: "StudentRollNumber");

            migrationBuilder.RenameColumn(
                name: "StudentClass",
                table: "Complaints",
                newName: "StudentDivision");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentRollNumber",
                table: "Complaints",
                newName: "StudentName");

            migrationBuilder.RenameColumn(
                name: "StudentDivision",
                table: "Complaints",
                newName: "StudentClass");
        }
    }
}
