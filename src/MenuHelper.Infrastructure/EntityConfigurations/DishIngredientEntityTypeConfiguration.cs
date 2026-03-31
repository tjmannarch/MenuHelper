using MenuHelper.Domain.AggregatesModel.DishAggregate;

namespace MenuHelper.Infrastructure.EntityConfigurations;

public class DishIngredientEntityTypeConfiguration : IEntityTypeConfiguration<DishIngredient>
{
    public void Configure(EntityTypeBuilder<DishIngredient> builder)
    {
        builder.ToTable("DishIngredients");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseGuidVersion7ValueGenerator()
            .HasComment("菜品原材料关联标识");

        builder.Property(x => x.IngredientId)
            .IsRequired()
            .HasComment("原材料标识");

        builder.Property(x => x.QuantityType)
            .IsRequired()
            .HasComment("用量类型：1=固定 2=非固定");

        builder.Property(x => x.FixedQuantity)
            .HasPrecision(18, 4)
            .HasComment("固定用量（QuantityType=固定时有效）");

        builder.HasIndex(x => x.IngredientId);
    }
}
