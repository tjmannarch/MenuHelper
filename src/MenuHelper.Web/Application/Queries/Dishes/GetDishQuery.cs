using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MenuHelper.Web.Application.Queries.Dishes;

public record GetDishQuery(DishId DishId) : IQuery<DishDetailDto>;

public class GetDishQueryValidator : AbstractValidator<GetDishQuery>
{
    public GetDishQueryValidator()
    {
        RuleFor(x => x.DishId).NotEmpty();
    }
}

public class GetDishQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetDishQuery, DishDetailDto>
{
    public async Task<DishDetailDto> Handle(GetDishQuery request, CancellationToken cancellationToken)
    {
        return await context.Dishes
            .Where(x => x.Id == request.DishId && !x.Deleted)
            .Select(x => new DishDetailDto(
                x.Id,
                x.Name,
                x.DishIngredients.Select(di => new DishIngredientDto(
                    di.Id, di.IngredientId, di.QuantityType, di.FixedQuantity)).ToList()))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KnownException($"未找到菜品，DishId = {request.DishId}");
    }
}

public record DishDetailDto(DishId Id, string Name, List<DishIngredientDto> DishIngredients);
public record DishIngredientDto(DishIngredientId Id, IngredientId IngredientId, QuantityType QuantityType, decimal? FixedQuantity);
