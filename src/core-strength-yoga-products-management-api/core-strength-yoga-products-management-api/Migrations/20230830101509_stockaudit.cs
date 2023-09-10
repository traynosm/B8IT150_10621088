using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace core_strength_yoga_products_api.Migrations
{
    /// <inheritdoc />
    public partial class stockaudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChangedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductAttributeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductAttributesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    OldStockLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    NewStockLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: true),
                    StockLevelChange = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockAudits_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockAudits_ProductAttributes_ProductAttributesId",
                        column: x => x.ProductAttributesId,
                        principalTable: "ProductAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockAudits_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockAudits");
        }
    }
}
