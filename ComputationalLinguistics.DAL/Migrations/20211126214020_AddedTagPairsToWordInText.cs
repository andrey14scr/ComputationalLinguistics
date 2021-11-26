using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComputationalLinguistics.DAL.Migrations
{
    public partial class AddedTagPairsToWordInText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TagPairId",
                table: "WordsInText",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WordsInText_TagPairId",
                table: "WordsInText",
                column: "TagPairId");

            migrationBuilder.AddForeignKey(
                name: "FK_WordsInText_TagPairs_TagPairId",
                table: "WordsInText",
                column: "TagPairId",
                principalTable: "TagPairs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordsInText_TagPairs_TagPairId",
                table: "WordsInText");

            migrationBuilder.DropIndex(
                name: "IX_WordsInText_TagPairId",
                table: "WordsInText");

            migrationBuilder.DropColumn(
                name: "TagPairId",
                table: "WordsInText");
        }
    }
}
