using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitterApi.Migrations
{
    /// <inheritdoc />
    public partial class RenameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tweet_Users_UserId",
                table: "Tweet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tweet",
                table: "Tweet");

            migrationBuilder.RenameTable(
                name: "Tweet",
                newName: "Tweets");

            migrationBuilder.RenameIndex(
                name: "IX_Tweet_UserId",
                table: "Tweets",
                newName: "IX_Tweets_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tweets",
                table: "Tweets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tweets_Users_UserId",
                table: "Tweets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tweets_Users_UserId",
                table: "Tweets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tweets",
                table: "Tweets");

            migrationBuilder.RenameTable(
                name: "Tweets",
                newName: "Tweet");

            migrationBuilder.RenameIndex(
                name: "IX_Tweets_UserId",
                table: "Tweet",
                newName: "IX_Tweet_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tweet",
                table: "Tweet",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tweet_Users_UserId",
                table: "Tweet",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
