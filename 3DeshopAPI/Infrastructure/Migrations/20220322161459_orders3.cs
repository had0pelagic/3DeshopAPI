using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class orders3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_ClientIdId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ClientIdId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ClientIdId",
                table: "Orders",
                newName: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Orders",
                newName: "ClientIdId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ClientIdId",
                table: "Orders",
                column: "ClientIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_ClientIdId",
                table: "Orders",
                column: "ClientIdId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
