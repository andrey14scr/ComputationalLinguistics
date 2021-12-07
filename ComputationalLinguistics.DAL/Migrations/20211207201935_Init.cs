using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComputationalLinguistics.DAL.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TagsInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagName = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    Info = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    IsGeneric = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagsInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    FileAnnotationPath = table.Column<string>(type: "nvarchar(220)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(120)", nullable: false),
                    Initial = table.Column<string>(type: "nvarchar(120)", nullable: true),
                    TagInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Words_TagsInfo_TagInfoId",
                        column: x => x.TagInfoId,
                        principalTable: "TagsInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WordsInText",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TextFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OffSet = table.Column<int>(type: "int", nullable: false),
                    WordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NextWordInTextId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordsInText", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordsInText_TextFiles_TextFileId",
                        column: x => x.TextFileId,
                        principalTable: "TextFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordsInText_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordsInText_WordsInText_NextWordInTextId",
                        column: x => x.NextWordInTextId,
                        principalTable: "WordsInText",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ITagName",
                table: "TagsInfo",
                column: "TagName");

            migrationBuilder.CreateIndex(
                name: "IFilePath",
                table: "TextFiles",
                column: "FilePath");

            migrationBuilder.CreateIndex(
                name: "IWordInfo",
                table: "Words",
                columns: new[] { "Content", "TagInfoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Words_TagInfoId",
                table: "Words",
                column: "TagInfoId");

            migrationBuilder.CreateIndex(
                name: "ITextFileId",
                table: "WordsInText",
                columns: new[] { "TextFileId", "OffSet" });

            migrationBuilder.CreateIndex(
                name: "IWordId",
                table: "WordsInText",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_WordsInText_NextWordInTextId",
                table: "WordsInText",
                column: "NextWordInTextId");

            var sql = $"INSERT INTO TagsInfo VALUES " +
                $"('{Guid.NewGuid()}', 'CC', 'coordinating conjunction', 'true')," +
                $"('{Guid.NewGuid()}', 'CD', 'cardinal digit', 'true')," +
                $"('{Guid.NewGuid()}', 'DT', 'determiner', 'true')," +
                $"('{Guid.NewGuid()}', 'EX', 'existential there', 'true')," +
                $"('{Guid.NewGuid()}', 'FW', 'foreign word', 'true')," +
                $"('{Guid.NewGuid()}', 'IN', 'preposition/subordinating conjunction', 'true')," +
                $"('{Guid.NewGuid()}', 'JJ', 'adjective', 'true')," +
                $"('{Guid.NewGuid()}', 'JJR', 'adjective, comparative', 'true')," +
                $"('{Guid.NewGuid()}', 'JJS', 'adjective, superlative', 'true')," +
                $"('{Guid.NewGuid()}', 'LS', 'list marker', 'true')," +
                $"('{Guid.NewGuid()}', 'MD', 'modal', 'true')," +
                $"('{Guid.NewGuid()}', 'NN', 'noun, singular', 'true')," +
                $"('{Guid.NewGuid()}', 'NNS', 'noun plural', 'true')," +
                $"('{Guid.NewGuid()}', 'NNP', 'proper noun, singular', 'true')," +
                $"('{Guid.NewGuid()}', 'NNPS', 'proper noun, plural', 'true')," +
                $"('{Guid.NewGuid()}', 'PDT', 'predeterminer', 'true')," +
                $"('{Guid.NewGuid()}', 'POS', 'possessive ending', 'true')," +
                $"('{Guid.NewGuid()}', 'PRP', 'personal pronoun', 'true')," +
                $"('{Guid.NewGuid()}', 'PRP$', 'possessive pronoun', 'true')," +
                $"('{Guid.NewGuid()}', 'RB', 'adverb', 'true')," +
                $"('{Guid.NewGuid()}', 'RBR', 'adverb, comparative', 'true')," +
                $"('{Guid.NewGuid()}', 'RBS', 'adverb, superlative', 'true')," +
                $"('{Guid.NewGuid()}', 'RP', 'particle', 'true')," +
                $"('{Guid.NewGuid()}', 'TO', 'to', 'true')," +
                $"('{Guid.NewGuid()}', 'UH', 'interjection', 'true')," +
                $"('{Guid.NewGuid()}', 'VB', 'verb, base form', 'true')," +
                $"('{Guid.NewGuid()}', 'VBD', 'verb, past tense', 'true')," +
                $"('{Guid.NewGuid()}', 'VBG', 'verb, gerund/present participle', 'true')," +
                $"('{Guid.NewGuid()}', 'VBN', 'verb, past participle', 'true')," +
                $"('{Guid.NewGuid()}', 'VBP', 'verb, sing. present, non-3d', 'true')," +
                $"('{Guid.NewGuid()}', 'VBZ', 'verb, 3rd person sing. present', 'true')," +
                $"('{Guid.NewGuid()}', 'WDT', 'wh-determiner', 'true')," +
                $"('{Guid.NewGuid()}', 'WP', 'wh-pronoun', 'true')," +
                $"('{Guid.NewGuid()}', 'WP$', 'possessive wh-pronoun', 'true')," +
                $"('{Guid.NewGuid()}', 'WRB', 'wh-abverb', 'true')";

            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordsInText");

            migrationBuilder.DropTable(
                name: "TextFiles");

            migrationBuilder.DropTable(
                name: "Words");

            migrationBuilder.DropTable(
                name: "TagsInfo");
        }
    }
}
