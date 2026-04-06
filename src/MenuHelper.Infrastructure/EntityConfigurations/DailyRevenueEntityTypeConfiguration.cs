using MenuHelper.Domain.AggregatesModel.DailyRevenueAggregate;
namespace MenuHelper.Infrastructure.EntityConfigurations;

public class DailyRevenueEntityTypeConfiguration : IEntityTypeConfiguration<DailyRevenue>
{
    public void Configure(EntityTypeBuilder<DailyRevenue> builder)
    {
        builder.ToTable("DailyRevenues");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseGuidVersion7ValueGenerator().HasComment("主键");
        builder.Property(x => x.Date).IsRequired().HasComment("营业日期");
        builder.Property(x => x.Amount).IsRequired().HasPrecision(18, 2).HasComment("营业额（元）");
        builder.Property(x => x.Note).HasMaxLength(500).HasComment("备注");
        builder.HasIndex(x => x.Date).IsUnique();
    }
}
