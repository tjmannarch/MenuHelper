using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace MenuHelper.Web.Application.Queries.Finance;

public record GetPurchaseStatsByCategoryQuery(DateOnly From, DateOnly To) : IQuery<List<CategoryStatDto>>;
public class GetPurchaseStatsByCategoryQueryValidator : AbstractValidator<GetPurchaseStatsByCategoryQuery> { }

public class GetPurchaseStatsByCategoryQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetPurchaseStatsByCategoryQuery, List<CategoryStatDto>>
{
    public async Task<List<CategoryStatDto>> Handle(GetPurchaseStatsByCategoryQuery q, CancellationToken ct)
    {
        var items = await context.Purchases
            .Where(p => !p.Deleted && p.PurchaseDate >= q.From && p.PurchaseDate <= q.To)
            .SelectMany(p => p.Items)
            .ToListAsync(ct);

        var ingredientIds = items.Select(i => i.IngredientId).Distinct().ToList();

        var categories = await context.Ingredients
            .Where(i => ingredientIds.Contains(i.Id))
            .Select(i => new { i.Id, i.Category })
            .ToListAsync(ct);

        var categoryMap = categories.ToDictionary(x => x.Id, x => x.Category);

        return items
            .GroupBy(i => categoryMap.TryGetValue(i.IngredientId, out var cat) ? cat : IngredientCategory.Common)
            .Select(g => new CategoryStatDto(g.Key.ToString(), GetCategoryName(g.Key), g.Sum(i => i.Quantity * i.UnitPrice)))
            .OrderByDescending(x => x.TotalAmount)
            .ToList();
    }

    private static string GetCategoryName(IngredientCategory cat) => cat switch
    {
        IngredientCategory.LiangPi => "凉皮类",
        IngredientCategory.RouJiaMo => "肉夹馍类",
        IngredientCategory.ShiGuoFan => "石锅饭类",
        IngredientCategory.Common => "通用食材",
        _ => "其他"
    };
}

public record CategoryStatDto(string Category, string CategoryName, decimal TotalAmount);
