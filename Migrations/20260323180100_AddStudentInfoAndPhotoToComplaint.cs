using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartGrievancePortal.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentInfoAndPhotoToComplaint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentClass",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentCourse",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentYear",
                table: "Complaints",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "StudentClass",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "StudentCourse",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "StudentName",
                table: "Complaints");

            migrationBuilder.DropColumn(
                name: "StudentYear",
                table: "Complaints");
        }
    }
}
