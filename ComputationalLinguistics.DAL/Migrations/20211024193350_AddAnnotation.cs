using Microsoft.EntityFrameworkCore.Migrations;

namespace ComputationalLinguistics.DAL.Migrations
{
    public partial class AddAnnotation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IContent",
                table: "Words");

            migrationBuilder.RenameColumn(
                name: "Seek",
                table: "WordsInText",
                newName: "OffSet");

            migrationBuilder.AddColumn<string>(
                name: "Annotation",
                table: "Words",
                type: "nvarchar(10)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IWordInfo",
                table: "Words",
                columns: new[] { "Content", "Annotation" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IWordInfo",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "Annotation",
                table: "Words");

            migrationBuilder.RenameColumn(
                name: "OffSet",
                table: "WordsInText",
                newName: "Seek");

            migrationBuilder.CreateIndex(
                name: "IContent",
                table: "Words",
                column: "Content");
        }
    }
}
