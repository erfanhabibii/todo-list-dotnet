using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTodoOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "todos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_todos_OwnerId_IsDone_CreatedAtUtc",
                table: "todos",
                columns: new[] { "OwnerId", "IsDone", "CreatedAtUtc" });

            migrationBuilder.AddForeignKey(
                name: "FK_todos_AspNetUsers_OwnerId",
                table: "todos",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_todos_AspNetUsers_OwnerId",
                table: "todos");

            migrationBuilder.DropIndex(
                name: "IX_todos_OwnerId_IsDone_CreatedAtUtc",
                table: "todos");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "todos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
