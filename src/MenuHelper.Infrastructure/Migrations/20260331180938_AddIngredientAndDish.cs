using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientAndDish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dishes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "菜品标识", collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "菜品名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Deleted = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否已删除"),
                    RowVersion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dishes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ingredients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "原材料标识", collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "原材料名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Unit = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "计量单位")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Category = table.Column<int>(type: "int", nullable: false, comment: "菜品分类：1=凉皮类 2=肉夹馍类 3=石锅饭类 4=通用食材"),
                    ConsumptionType = table.Column<int>(type: "int", nullable: false, comment: "消耗方式：1=即时消耗 2=摊销消耗"),
                    SupplierId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "供应商标识，null表示自购", collation: "ascii_general_ci"),
                    SafetyStockLevel = table.Column<decimal>(type: "decimal(65,30)", nullable: true, comment: "安全库存线"),
                    RestockCycleDays = table.Column<int>(type: "int", nullable: true, comment: "默认备货周期（天）"),
                    MaxShelfDays = table.Column<int>(type: "int", nullable: true, comment: "最长存放天数"),
                    DefaultUnitPrice = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false, comment: "默认单价（预设基准值）"),
                    Deleted = table.Column<bool>(type: "tinyint(1)", nullable: false, comment: "是否已删除"),
                    RowVersion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredients", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DishIngredients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "菜品原材料关联标识", collation: "ascii_general_ci"),
                    IngredientId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "原材料标识", collation: "ascii_general_ci"),
                    QuantityType = table.Column<int>(type: "int", nullable: false, comment: "用量类型：1=固定 2=非固定"),
                    FixedQuantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: true, comment: "固定用量（QuantityType=固定时有效）"),
                    DishId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishIngredients_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_Deleted",
                table: "Dishes",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_DishIngredients_DishId",
                table: "DishIngredients",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishIngredients_IngredientId",
                table: "DishIngredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Category",
                table: "Ingredients",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Deleted",
                table: "Ingredients",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Name",
                table: "Ingredients",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishIngredients");

            migrationBuilder.DropTable(
                name: "Ingredients");

            migrationBuilder.DropTable(
                name: "Dishes");
        }
    }
}
