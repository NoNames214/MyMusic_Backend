using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicApi.Migrations
{
    /// <inheritdoc />
    public partial class RecreateFavorite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    UserId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    SongId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    LikeAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    isDeleted = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    DeleteAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    DeletedBy = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);

                    table.ForeignKey(
                        name: "FK_Favorites_Songs_SongId",
                        column: x => x.SongId,
                        principalTable: "Songs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_Favorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId_SongId",
                table: "Favorites",
                columns: new[] { "UserId", "SongId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorites");
        }
    }
}
