using MenuHelper.Domain.AggregatesModel.DishAggregate;

namespace MenuHelper.Infrastructure.EntityConfigurations;

public class DishEntityTypeConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.ToTable("Dishes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .UseGuidVersion7ValueGenerator()
            .HasComment("菜品标识");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("菜品名称");

        builder.Property(x => x.Deleted)
            .IsRequired()
            .HasComment("是否已删除");

        builder.HasMany(x => x.DishIngredients)
            .WithOne()
            .HasForeignKey("DishId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Deleted);
    }
}
