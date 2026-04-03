using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MenuHelper.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConsumptionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ConsumptionType",
                table: "Ingredients",
                type: "int",
                nullable: false,
                comment: "消耗方式：1=按库存盘点 2=按周期摊销 3=按销量推算",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "消耗方式：1=即时消耗 2=摊销消耗");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ConsumptionType",
                table: "Ingredients",
                type: "int",
                nullable: false,
                comment: "消耗方式：1=即时消耗 2=摊销消耗",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "消耗方式：1=按库存盘点 2=按周期摊销 3=按销量推算");
        }
    }
}
