using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace core_strength_yoga_products_api.Migrations
{
    /// <inheritdoc />
    public partial class amendstockorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockAudits_Orders_OrderId",
                table: "StockAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAudits_ProductAttributes_ProductAttributesId",
                table: "StockAudits");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAudits_Products_ProductId",
                table: "StockAudits");

            migrationBuilder.DropIndex(
                name: "IX_StockAudits_OrderId",
                table: "StockAudits");

            migrationBuilder.DropIndex(
                name: "IX_StockAudits_ProductAttributesId",
                table: "StockAudits");

            migrationBuilder.DropIndex(
                name: "IX_StockAudits_ProductId",
                table: "StockAudits");

            migrationBuilder.DropColumn(
                name: "ProductAttributesId",
                table: "StockAudits");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductAttributesId",
                table: "StockAudits",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StockAudits_OrderId",
                table: "StockAudits",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAudits_ProductAttributesId",
                table: "StockAudits",
                column: "ProductAttributesId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAudits_ProductId",
                table: "StockAudits",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockAudits_Orders_OrderId",
                table: "StockAudits",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockAudits_ProductAttributes_ProductAttributesId",
                table: "StockAudits",
                column: "ProductAttributesId",
                principalTable: "ProductAttributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockAudits_Products_ProductId",
                table: "StockAudits",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
