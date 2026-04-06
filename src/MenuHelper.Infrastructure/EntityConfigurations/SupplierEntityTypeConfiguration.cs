using MenuHelper.Domain.AggregatesModel.SupplierAggregate;

namespace MenuHelper.Infrastructure.EntityConfigurations;

public class SupplierEntityTypeConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseGuidVersion7ValueGenerator()
            .HasComment("供应商标识");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("供应商名称");

        builder.Property(x => x.Phone)
            .HasMaxLength(50)
            .HasComment("联系电话");

        builder.Property(x => x.Remark)
            .HasMaxLength(500)
            .HasComment("备注（负责品类说明等）");

        builder.Property(x => x.Deleted)
            .IsRequired()
            .HasComment("是否已删除");

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.Deleted);
    }
}
