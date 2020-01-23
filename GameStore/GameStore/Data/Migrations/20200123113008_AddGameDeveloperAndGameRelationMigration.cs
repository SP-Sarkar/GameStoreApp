using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class AddGameDeveloperAndGameRelationMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "gameDeveloper",
                table: "games",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_games_gameDeveloper",
                table: "games",
                column: "gameDeveloper");

            migrationBuilder.AddForeignKey(
                name: "FK_games_gameDev_gameDeveloper",
                table: "games",
                column: "gameDeveloper",
                principalTable: "gameDev",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_games_gameDev_gameDeveloper",
                table: "games");

            migrationBuilder.DropIndex(
                name: "IX_games_gameDeveloper",
                table: "games");

            migrationBuilder.DropColumn(
                name: "gameDeveloper",
                table: "games");
        }
    }
}
