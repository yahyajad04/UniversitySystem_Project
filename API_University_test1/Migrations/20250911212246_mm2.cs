using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_University_test1.Migrations
{
    /// <inheritdoc />
    public partial class mm2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Course_Cost",
                table: "Courses");

            migrationBuilder.AddColumn<int>(
                name: "Course_Hours",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Course_Hours",
                table: "Courses");

            migrationBuilder.AddColumn<double>(
                name: "Course_Cost",
                table: "Courses",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
