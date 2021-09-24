using Microsoft.EntityFrameworkCore.Migrations;

namespace ComputationalLinguistics.DAL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TextFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(200)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(120)", nullable: false),
                    Frequency = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WordsInText",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TextFileId = table.Column<int>(type: "int", nullable: false),
                    Seek = table.Column<int>(type: "int", nullable: false),
                    WordId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordsInText", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordsInText_TextFiles_TextFileId",
                        column: x => x.TextFileId,
                        principalTable: "TextFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WordsInText_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IFilePath",
                table: "TextFiles",
                column: "FilePath");

            migrationBuilder.CreateIndex(
                name: "IContent",
                table: "Words",
                column: "Content");

            migrationBuilder.CreateIndex(
                name: "IWordId",
                table: "WordsInText",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_WordsInText_TextFileId",
                table: "WordsInText",
                column: "TextFileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordsInText");

            migrationBuilder.DropTable(
                name: "TextFiles");

            migrationBuilder.DropTable(
                name: "Words");
        }
    }
}
