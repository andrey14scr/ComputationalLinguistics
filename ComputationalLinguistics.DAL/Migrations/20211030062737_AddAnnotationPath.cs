using Microsoft.EntityFrameworkCore.Migrations;

namespace ComputationalLinguistics.DAL.Migrations
{
    public partial class AddAnnotationPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileAnnotationPath",
                table: "TextFiles",
                type: "nvarchar(220)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileAnnotationPath",
                table: "TextFiles");
        }
    }
}
