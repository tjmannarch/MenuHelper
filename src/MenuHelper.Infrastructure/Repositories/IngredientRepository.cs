using MenuHelper.Domain.AggregatesModel.IngredientAggregate;

namespace MenuHelper.Infrastructure.Repositories;

public interface IIngredientRepository : IRepository<Ingredient, IngredientId>
{
    /// <summary>
    /// 检查原材料是否已被任何菜品关联，用于删除前的校验
    /// </summary>
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
