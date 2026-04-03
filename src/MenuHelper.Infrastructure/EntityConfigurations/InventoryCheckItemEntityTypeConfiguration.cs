using MenuHelper.Domain.AggregatesModel.InventoryCheckAggregate;

namespace MenuHelper.Infrastructure.EntityConfigurations;

public class InventoryCheckItemEntityTypeConfiguration : IEntityTypeConfiguration<InventoryCheckItem>
{
    public void Configure(EntityTypeBuilder<InventoryCheckItem> builder)
    {
        builder.ToTable("InventoryCheckItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseGuidVersion7ValueGenerator()
            .HasComment("库存盘点明细标识");

        builder.Property(x => x.IngredientId)
            .IsRequired()
            .HasComment("原材料标识");

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasPrecision(18, 4)
            .HasComment("剩余库存数量");

        builder.HasIndex(x => x.IngredientId);
    }
}
