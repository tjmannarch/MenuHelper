using MenuHelper.Domain.AggregatesModel.InventoryCheckAggregate;

namespace MenuHelper.Infrastructure.EntityConfigurations;

public class InventoryCheckEntityTypeConfiguration : IEntityTypeConfiguration<InventoryCheck>
{
    public void Configure(EntityTypeBuilder<InventoryCheck> builder)
    {
        builder.ToTable("InventoryChecks");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseGuidVersion7ValueGenerator()
            .HasComment("库存盘点标识");

        builder.Property(x => x.CheckDate)
            .IsRequired()
            .HasComment("盘点日期");

        builder.Property(x => x.Deleted)
            .IsRequired()
            .HasComment("是否已删除");

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey("InventoryCheckId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CheckDate).IsUnique();
        builder.HasIndex(x => x.Deleted);
    }
}
