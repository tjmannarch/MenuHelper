using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeDefaultUnitPriceNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultUnitPrice",
                table: "Ingredients",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                comment: "默认单价（预设基准值，选填）",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldComment: "默认单价（预设基准值）");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultUnitPrice",
                table: "Ingredients",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                comment: "默认单价（预设基准值）",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true,
                oldComment: "默认单价（预设基准值，选填）");
        }
    }
}
