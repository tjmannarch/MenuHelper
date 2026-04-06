using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace MenuHelper.Web.Application.Queries.Finance;

public record GetPurchaseStatsBySupplierQuery(DateOnly From, DateOnly To) : IQuery<List<SupplierStatDto>>;
public class GetPurchaseStatsBySupplierQueryValidator : AbstractValidator<GetPurchaseStatsBySupplierQuery> { }

public class GetPurchaseStatsBySupplierQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetPurchaseStatsBySupplierQuery, List<SupplierStatDto>>
{
    public async Task<List<SupplierStatDto>> Handle(GetPurchaseStatsBySupplierQuery q, CancellationToken ct)
    {
        var data = await context.Purchases
            .Where(p => !p.Deleted && p.PurchaseDate >= q.From && p.PurchaseDate <= q.To)
            .Select(p => new
            {
                p.SupplierId,
                p.SupplierName,
                TotalAmount = p.Items.Sum(i => i.Quantity * i.UnitPrice)
            })
            .ToListAsync(ct);

        return data
            .GroupBy(p => new { p.SupplierId, SupplierName = p.SupplierName ?? "自购" })
            .Select(g => new SupplierStatDto(
                g.Key.SupplierId?.ToString(),
                g.Key.SupplierName,
                g.Sum(p => p.TotalAmount)))
            .OrderByDescending(x => x.TotalAmount)
            .ToList();
    }
}

public record SupplierStatDto(string? SupplierId, string SupplierName, decimal TotalAmount);
