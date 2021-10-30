using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ComputationalLinguistics.DAL.Migrations
{
    public partial class AddTagInfoData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Annotation",
                table: "Words",
                newName: "Tag");

            var sql = $"INSERT INTO TagsInfo VALUES " +
                      $"('{Guid.NewGuid()}', 'CC', 'coordinating conjunction')," +
                      $"('{Guid.NewGuid()}', 'CD', 'cardinal digit')," +
                      $"('{Guid.NewGuid()}', 'DT', 'determiner')," +
                      $"('{Guid.NewGuid()}', 'EX', 'existential there')," +
                      $"('{Guid.NewGuid()}', 'FW', 'foreign word')," +
                      $"('{Guid.NewGuid()}', 'IN', 'preposition/subordinating conjunction')," +
                      $"('{Guid.NewGuid()}', 'JJ', 'adjective')," +
                      $"('{Guid.NewGuid()}', 'JJR', 'adjective, comparative')," +
                      $"('{Guid.NewGuid()}', 'JJS', 'adjective, superlative')," +
                      $"('{Guid.NewGuid()}', 'LS', 'list marker')," +
                      $"('{Guid.NewGuid()}', 'MD', 'modal')," +
                      $"('{Guid.NewGuid()}', 'NN', 'noun, singular')," +
                      $"('{Guid.NewGuid()}', 'NNS', 'noun plural')," +
                      $"('{Guid.NewGuid()}', 'NNP', 'proper noun, singular')," +
                      $"('{Guid.NewGuid()}', 'NNPS', 'proper noun, plural')," +
                      $"('{Guid.NewGuid()}', 'PDT', 'predeterminer')," +
                      $"('{Guid.NewGuid()}', 'POS', 'possessive ending')," +
                      $"('{Guid.NewGuid()}', 'PRP', 'personal pronoun')," +
                      $"('{Guid.NewGuid()}', 'PRP$', 'possessive pronoun')," +
                      $"('{Guid.NewGuid()}', 'RB', 'adverb')," +
                      $"('{Guid.NewGuid()}', 'RBR', 'adverb, comparative')," +
                      $"('{Guid.NewGuid()}', 'RBS', 'adverb, superlative')," +
                      $"('{Guid.NewGuid()}', 'RP', 'particle')," +
                      $"('{Guid.NewGuid()}', 'TO', 'to')," +
                      $"('{Guid.NewGuid()}', 'UH', 'interjection')," +
                      $"('{Guid.NewGuid()}', 'VB', 'verb, base form')," +
                      $"('{Guid.NewGuid()}', 'VBD', 'verb, past tense')," +
                      $"('{Guid.NewGuid()}', 'VBG', 'verb, gerund/present participle')," +
                      $"('{Guid.NewGuid()}', 'VBN', 'verb, past participle')," +
                      $"('{Guid.NewGuid()}', 'VBP', 'verb, sing. present, non-3d')," +
                      $"('{Guid.NewGuid()}', 'VBZ', 'verb, 3rd person sing. present')," +
                      $"('{Guid.NewGuid()}', 'WDT', 'wh-determiner')," +
                      $"('{Guid.NewGuid()}', 'WP', 'wh-pronoun')," +
                      $"('{Guid.NewGuid()}', 'WP$', 'possessive wh-pronoun')," +
                      $"('{Guid.NewGuid()}', 'WRB', 'wh-abverb')";

            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tag",
                table: "Words",
                newName: "Annotation");

            var sql = $"DELETE FROM TagsInfo";
            migrationBuilder.Sql(sql);
        }
    }
}
