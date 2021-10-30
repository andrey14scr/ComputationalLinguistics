using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComputationalLinguistics.DAL.Migrations
{
    public partial class AddTagHelp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TagsInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagName = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    Info = table.Column<string>(type: "nvarchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagsInfo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "ITagName",
                table: "TagsInfo",
                column: "TagName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagsInfo");
        }
    }
}
