using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_University_test1.Migrations
{
    /// <inheritdoc />
    public partial class lettergrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "letter_grade",
                table: "Grades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "letter_grade",
                table: "Grades");
        }
    }
}
