using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class AddKeyIntoGameCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_GameCategories_Id",
                table: "GameCategories");

            migrationBuilder.DropIndex(
                name: "IX_GameCategories_CategoryId",
                table: "GameCategories");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GameCategories_CategoryId_GameId_Id",
                table: "GameCategories",
                columns: new[] { "CategoryId", "GameId", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_GameCategories_CategoryId_GameId_Id",
                table: "GameCategories");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GameCategories_Id",
                table: "GameCategories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GameCategories_CategoryId",
                table: "GameCategories",
                column: "CategoryId");
        }
    }
}
