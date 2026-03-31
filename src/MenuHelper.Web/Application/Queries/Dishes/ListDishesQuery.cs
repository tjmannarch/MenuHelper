using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Infrastructure;

namespace MenuHelper.Web.Application.Queries.Dishes;

public record ListDishesQuery(
    string? Keyword = null,
    int PageIndex = 1,
    int PageSize = 20,
    bool CountTotal = true) : IPagedQuery<DishListItemDto>;

public class ListDishesQueryValidator : AbstractValidator<ListDishesQuery>
{
    public ListDishesQueryValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public class ListDishesQueryHandler(ApplicationDbContext context)
    : IQueryHandler<ListDishesQuery, PagedData<DishListItemDto>>
{
    public async Task<PagedData<DishListItemDto>> Handle(ListDishesQuery request, CancellationToken cancellationToken)
    {
        return await context.Dishes
            .Where(x => !x.Deleted)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Keyword), x => x.Name.Contains(request.Keyword!))
            .OrderBy(x => x.Name)
            .Select(x => new DishListItemDto(x.Id, x.Name, x.DishIngredients.Count))
            .ToPagedDataAsync(request, cancellationToken: cancellationToken);
    }
}

public record DishListItemDto(DishId Id, string Name, int IngredientCount);
