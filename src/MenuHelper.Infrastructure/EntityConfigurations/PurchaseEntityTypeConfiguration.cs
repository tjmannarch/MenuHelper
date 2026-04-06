using MenuHelper.Domain.AggregatesModel.PurchaseAggregate;

namespace MenuHelper.Infrastructure.EntityConfigurations;

public class PurchaseEntityTypeConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("Purchases");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseGuidVersion7ValueGenerator()
            .HasComment("进货记录标识");

        builder.Property(x => x.SupplierId)
            .HasComment("供应商标识，null表示自购");

        builder.Property(x => x.SupplierName)
            .HasMaxLength(100)
            .HasComment("供应商名称快照");

        builder.Property(x => x.PurchaseDate)
            .IsRequired()
            .HasComment("进货日期");

        builder.Property(x => x.IsPaid)
            .IsRequired()
            .HasComment("是否已结算");

        builder.Property(x => x.Remark)
            .HasMaxLength(500)
            .HasComment("备注");

        builder.Property(x => x.Deleted)
            .IsRequired()
            .HasComment("是否已删除");

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey("PurchaseId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.SupplierId);
        builder.HasIndex(x => x.PurchaseDate);
        builder.HasIndex(x => x.IsPaid);
        builder.HasIndex(x => x.Deleted);
    }
}
