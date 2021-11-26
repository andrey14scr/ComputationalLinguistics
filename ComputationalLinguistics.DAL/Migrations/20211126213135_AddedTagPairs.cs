using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComputationalLinguistics.DAL.Migrations
{
    public partial class AddedTagPairs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TagPairs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NextId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagPairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagPairs_TagsInfo_CurrentId",
                        column: x => x.CurrentId,
                        principalTable: "TagsInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TagPairs_TagsInfo_NextId",
                        column: x => x.NextId,
                        principalTable: "TagsInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagPairs_CurrentId",
                table: "TagPairs",
                column: "CurrentId");

            migrationBuilder.CreateIndex(
                name: "IX_TagPairs_NextId",
                table: "TagPairs",
                column: "NextId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagPairs");
        }
    }
}
