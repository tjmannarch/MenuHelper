using MenuHelper.Domain.AggregatesModel.IngredientAggregate;

namespace MenuHelper.Infrastructure.EntityConfigurations;

public class IngredientEntityTypeConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("Ingredients");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseGuidVersion7ValueGenerator()
            .HasComment("原材料标识");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("原材料名称");

        builder.Property(x => x.Unit)
            .IsRequired()
            .HasMaxLength(20)
            .HasComment("计量单位");

        builder.Property(x => x.Category)
            .IsRequired()
            .HasComment("菜品分类：1=凉皮类 2=肉夹馍类 3=石锅饭类 4=通用食材");

        builder.Property(x => x.ConsumptionType)
            .IsRequired()
            .HasComment("消耗方式：1=即时消耗 2=摊销消耗");

        builder.Property(x => x.SupplierId)
            .HasComment("供应商标识，null表示自购");

        builder.Property(x => x.SafetyStockLevel)
            .HasComment("安全库存线");

        builder.Property(x => x.RestockCycleDays)
            .HasComment("默认备货周期（天）");

        builder.Property(x => x.MaxShelfDays)
            .HasComment("最长存放天数");

        builder.Property(x => x.DefaultUnitPrice)
            .IsRequired()
            .HasPrecision(18, 4)
            .HasComment("默认单价（预设基准值）");

        builder.Property(x => x.Deleted)
            .IsRequired()
            .HasComment("是否已删除");

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.Category);
        builder.HasIndex(x => x.Deleted);
    }
}
