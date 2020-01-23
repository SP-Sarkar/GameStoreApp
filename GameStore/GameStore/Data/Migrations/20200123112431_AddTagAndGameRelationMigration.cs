using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class AddTagAndGameRelationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "gameTag",
                table: "games",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_games_gameTag",
                table: "games",
                column: "gameTag");

            migrationBuilder.AddForeignKey(
                name: "FK_games_tags_gameTag",
                table: "games",
                column: "gameTag",
                principalTable: "tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_games_tags_gameTag",
                table: "games");

            migrationBuilder.DropIndex(
                name: "IX_games_gameTag",
                table: "games");

            migrationBuilder.DropColumn(
                name: "gameTag",
                table: "games");
        }
    }
}
