using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "库存盘点标识", collation: "ascii_general_ci"),
                    CheckDate = table.Column<DateOnly>(type: "date", nullable: false, comment: "盘点日期"),
                    Deleted = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否已删除"),
                    RowVersion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryChecks", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventoryCheckItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "库存盘点明细标识", collation: "ascii_general_ci"),
                    IngredientId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "原材料标识", collation: "ascii_general_ci"),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, comment: "剩余库存数量"),
                    InventoryCheckId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCheckItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCheckItems_InventoryChecks_InventoryCheckId",
                        column: x => x.InventoryCheckId,
                        principalTable: "InventoryChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheckItems_IngredientId",
                table: "InventoryCheckItems",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheckItems_InventoryCheckId",
                table: "InventoryCheckItems",
                column: "InventoryCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryChecks_CheckDate",
                table: "InventoryChecks",
                column: "CheckDate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryChecks_Deleted",
                table: "InventoryChecks",
                column: "Deleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryCheckItems");

            migrationBuilder.DropTable(
                name: "InventoryChecks");
        }
    }
}
