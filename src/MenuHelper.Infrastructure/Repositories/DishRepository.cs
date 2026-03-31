using MenuHelper.Domain.AggregatesModel.DishAggregate;

namespace MenuHelper.Infrastructure.Repositories;

public interface IDishRepository : IRepository<Dish, DishId>
{
}

public class DishRepository(ApplicationDbContext context)
    : RepositoryBase<Dish, DishId, ApplicationDbContext>(context), IDishRepository
{
}
