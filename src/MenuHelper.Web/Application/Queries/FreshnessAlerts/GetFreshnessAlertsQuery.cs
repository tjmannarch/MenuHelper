using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace MenuHelper.Web.Application.Queries.FreshnessAlerts;

public record GetFreshnessAlertsQuery : IQuery<List<FreshnessAlertDto>>;
public class GetFreshnessAlertsQueryValidator : AbstractValidator<GetFreshnessAlertsQuery> { }

public class GetFreshnessAlertsQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetFreshnessAlertsQuery, List<FreshnessAlertDto>>
{
    public async Task<List<FreshnessAlertDto>> Handle(GetFreshnessAlertsQuery q, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var ingredients = await context.Ingredients
            .Where(i => !i.Deleted && i.MaxShelfDays.HasValue)
            .Select(i => new { i.Id, i.Name, i.Unit, i.MaxShelfDays })
            .ToListAsync(ct);

        if (!ingredients.Any()) return [];

        var ingredientIds = ingredients.Select(i => i.Id).ToList();

        var lastPurchases = await context.Purchases
            .Where(p => !p.Deleted)
            .SelectMany(p => p.Items.Select(item => new { item.IngredientId, p.PurchaseDate }))
            .Where(x => ingredientIds.Contains(x.IngredientId))
            .GroupBy(x => x.IngredientId)
            .Select(g => new { IngredientId = g.Key, LastPurchaseDate = g.Max(x => x.PurchaseDate) })
            .ToListAsync(ct);

        var lastPurchaseMap = lastPurchases.ToDictionary(x => x.IngredientId, x => x.LastPurchaseDate);

        return ingredients
            .Select(i =>
            {
                if (!lastPurchaseMap.TryGetValue(i.Id, out var lastDate))
                    return null;
                int daysStored = today.DayNumber - lastDate.DayNumber;
                if (daysStored <= i.MaxShelfDays!.Value)
                    return null;
                return new FreshnessAlertDto(
                    i.Id.ToString(),
                    i.Name,
                    i.Unit,
                    lastDate.ToString("yyyy-MM-dd"),
                    daysStored,
                    i.MaxShelfDays.Value);
            })
            .Where(x => x is not null)
            .Cast<FreshnessAlertDto>()
            .OrderByDescending(x => x.DaysStored - x.MaxShelfDays)
            .ToList();
    }
}

public record FreshnessAlertDto(
    string IngredientId,
    string IngredientName,
    string Unit,
    string LastPurchaseDate,
    int DaysStored,
    int MaxShelfDays);
