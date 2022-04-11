using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class balanceproductorderimplementation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "BalanceHistory",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "BalanceHistory",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BalanceHistory_OrderId",
                table: "BalanceHistory",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceHistory_ProductId",
                table: "BalanceHistory",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceHistory_Orders_OrderId",
                table: "BalanceHistory",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceHistory_Products_ProductId",
                table: "BalanceHistory",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceHistory_Orders_OrderId",
                table: "BalanceHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_BalanceHistory_Products_ProductId",
                table: "BalanceHistory");

            migrationBuilder.DropIndex(
                name: "IX_BalanceHistory_OrderId",
                table: "BalanceHistory");

            migrationBuilder.DropIndex(
                name: "IX_BalanceHistory_ProductId",
                table: "BalanceHistory");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "BalanceHistory");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "BalanceHistory");
        }
    }
}
