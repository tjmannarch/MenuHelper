using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace MenuHelper.Web.Application.Queries.Finance;

public record GetPurchaseStatsByIngredientQuery(DateOnly From, DateOnly To) : IQuery<List<IngredientStatDto>>;
public class GetPurchaseStatsByIngredientQueryValidator : AbstractValidator<GetPurchaseStatsByIngredientQuery> { }

public class GetPurchaseStatsByIngredientQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetPurchaseStatsByIngredientQuery, List<IngredientStatDto>>
{
    public async Task<List<IngredientStatDto>> Handle(GetPurchaseStatsByIngredientQuery q, CancellationToken ct)
    {
        return await context.Purchases
            .Where(p => !p.Deleted && p.PurchaseDate >= q.From && p.PurchaseDate <= q.To)
            .SelectMany(p => p.Items)
            .GroupBy(i => new { i.IngredientId, i.IngredientName, i.Unit })
            .Select(g => new IngredientStatDto(
                g.Key.IngredientId.ToString(),
                g.Key.IngredientName,
                g.Key.Unit,
                g.Sum(i => i.Quantity),
                g.Sum(i => i.Quantity * i.UnitPrice)))
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync(ct);
    }
}

public record IngredientStatDto(string IngredientId, string IngredientName, string Unit, decimal TotalQuantity, decimal TotalAmount);
