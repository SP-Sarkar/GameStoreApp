using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameStore.Data.Migrations
{
    public partial class AddGameTableToDatbaseMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GuidValue = table.Column<Guid>(nullable: false),
                    CTime = table.Column<DateTime>(nullable: false),
                    UTime = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    gameName = table.Column<string>(maxLength: 200, nullable: false),
                    gameUrl = table.Column<string>(maxLength: 200, nullable: true),
                    gamePrice = table.Column<decimal>(nullable: false),
                    gameDesc = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_games", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "games");
        }
    }
}
