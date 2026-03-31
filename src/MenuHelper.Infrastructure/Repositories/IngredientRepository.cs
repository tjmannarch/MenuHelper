using MenuHelper.Domain.AggregatesModel.IngredientAggregate;

namespace MenuHelper.Infrastructure.Repositories;

public interface IIngredientRepository : IRepository<Ingredient, IngredientId>
{
    Task<bool> IsIngredientInUseAsync(IngredientId ingredientId, CancellationToken cancellationToken = default);
}

public class IngredientRepository(ApplicationDbContext context)
    : RepositoryBase<Ingredient, IngredientId, ApplicationDbContext>(context), IIngredientRepository
{
    public async Task<bool> IsIngredientInUseAsync(IngredientId ingredientId, CancellationToken cancellationToken = default)
    {
        return await DbContext.DishIngredients.AnyAsync(x => x.IngredientId == ingredientId, cancellationToken);
    }
}
