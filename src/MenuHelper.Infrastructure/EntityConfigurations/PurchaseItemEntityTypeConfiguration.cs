using MenuHelper.Domain.AggregatesModel.PurchaseAggregate;

namespace MenuHelper.Infrastructure.EntityConfigurations;

public class PurchaseItemEntityTypeConfiguration : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.ToTable("PurchaseItems");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseGuidVersion7ValueGenerator()
            .HasComment("进货明细标识");

        builder.Property(x => x.IngredientId)
            .IsRequired()
            .HasComment("原材料标识");

        builder.Property(x => x.IngredientName)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("原材料名称快照");

        builder.Property(x => x.Unit)
            .IsRequired()
            .HasMaxLength(20)
            .HasComment("计量单位快照");

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasPrecision(18, 4)
            .HasComment("进货数量");

        builder.Property(x => x.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 4)
            .HasComment("进货单价（价格快照，历史永久不变）");

        builder.HasIndex(x => x.IngredientId);
    }
}
