using MenuHelper.Domain.AggregatesModel.DishAggregate;

namespace MenuHelper.Infrastructure.Repositories;

public interface IDishRepository : IRepository<Dish, DishId>
{
    Task<Dish?> GetWithIngredientsAsync(DishId id, CancellationToken cancellationToken = default);
}

public class DishRepository(ApplicationDbContext context)
    : RepositoryBase<Dish, DishId, ApplicationDbContext>(context), IDishRepository
{
    public async Task<Dish?> GetWithIngredientsAsync(DishId id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Dishes
            .Include(x => x.DishIngredients)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
