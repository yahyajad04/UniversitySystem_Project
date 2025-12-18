using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_University_test1.Migrations
{
    /// <inheritdoc />
    public partial class addedthemajortocourserelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoursesMajors",
                columns: table => new
                {
                    CoursesId = table.Column<int>(type: "int", nullable: false),
                    MajorsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursesMajors", x => new { x.CoursesId, x.MajorsId });
                    table.ForeignKey(
                        name: "FK_CoursesMajors_Courses_CoursesId",
                        column: x => x.CoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursesMajors_Majors_MajorsId",
                        column: x => x.MajorsId,
                        principalTable: "Majors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursesMajors_MajorsId",
                table: "CoursesMajors",
                column: "MajorsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursesMajors");
        }
    }
}
