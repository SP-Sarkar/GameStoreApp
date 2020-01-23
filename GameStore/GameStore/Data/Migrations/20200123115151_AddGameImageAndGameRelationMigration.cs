using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class AddGameImageAndGameRelationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "gameImages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_gameImages_GameId",
                table: "gameImages",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_gameImages_games_GameId",
                table: "gameImages",
                column: "GameId",
                principalTable: "games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_gameImages_games_GameId",
                table: "gameImages");

            migrationBuilder.DropIndex(
                name: "IX_gameImages_GameId",
                table: "gameImages");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "gameImages");
        }
    }
}
