using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MenuHelper.Web.Application.Queries.Ingredients;

public record ListIngredientsQuery(
    IngredientCategory? Category = null,
    ConsumptionType? ConsumptionType = null,
    string? Keyword = null,
    int PageIndex = 1,
    int PageSize = 20,
    bool CountTotal = true) : IPagedQuery<IngredientListItemDto>;

public class ListIngredientsQueryValidator : AbstractValidator<ListIngredientsQuery>
{
    public ListIngredientsQueryValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public class ListIngredientsQueryHandler(ApplicationDbContext context)
    : IQueryHandler<ListIngredientsQuery, PagedData<IngredientListItemDto>>
{
    public async Task<PagedData<IngredientListItemDto>> Handle(ListIngredientsQuery request, CancellationToken cancellationToken)
    {
        return await context.Ingredients
            .Where(x => !x.Deleted)
            .WhereIf(request.Category.HasValue, x => x.Category == request.Category!.Value)
            .WhereIf(request.ConsumptionType.HasValue, x => x.ConsumptionType == request.ConsumptionType!.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Keyword), x => x.Name.Contains(request.Keyword!))
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Name)
            .Select(x => new IngredientListItemDto(
                x.Id, x.Name, x.Unit, x.Category, x.ConsumptionType,
                x.SupplierId, x.SafetyStockLevel, x.DefaultUnitPrice))
            .ToPagedDataAsync(request, cancellationToken: cancellationToken);
    }
}

public record IngredientListItemDto(
    IngredientId Id,
    string Name,
    string Unit,
    IngredientCategory Category,
    ConsumptionType ConsumptionType,
    SupplierId? SupplierId,
    decimal? SafetyStockLevel,
    decimal DefaultUnitPrice);
