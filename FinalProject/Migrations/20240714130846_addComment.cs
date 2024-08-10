using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Migrations
{
    public partial class addComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentedPostId",
                table: "Posts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CommentedPostId",
                table: "Posts",
                column: "CommentedPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Posts_CommentedPostId",
                table: "Posts",
                column: "CommentedPostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Posts_CommentedPostId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CommentedPostId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CommentedPostId",
                table: "Posts");
        }
    }
}
