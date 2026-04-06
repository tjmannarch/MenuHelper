using MenuHelper.Domain.AggregatesModel.PurchaseAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MenuHelper.Web.Application.Queries.Purchases;

public record GetSupplierBalanceSummaryQuery : IQuery<List<SupplierBalanceDto>>;

public class GetSupplierBalanceSummaryQueryValidator : AbstractValidator<GetSupplierBalanceSummaryQuery>
{
    // no validation needed
}

public class GetSupplierBalanceSummaryQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetSupplierBalanceSummaryQuery, List<SupplierBalanceDto>>
{
    public async Task<List<SupplierBalanceDto>> Handle(GetSupplierBalanceSummaryQuery request, CancellationToken cancellationToken)
    {
        var data = await context.Set<Purchase>()
            .Where(p => !p.Deleted && !p.IsPaid && p.SupplierId != null)
            .Select(p => new
            {
                p.SupplierId,
                p.SupplierName,
                TotalAmount = p.Items.Sum(i => i.Quantity * i.UnitPrice)
            })
            .ToListAsync(cancellationToken);

        return data
            .GroupBy(p => new { p.SupplierId, p.SupplierName })
            .Select(g => new SupplierBalanceDto(
                g.Key.SupplierId!,
                g.Key.SupplierName ?? string.Empty,
                g.Sum(p => p.TotalAmount),
                g.Count()))
            .ToList();
    }
}

public record SupplierBalanceDto(
    SupplierId SupplierId,
    string SupplierName,
    decimal OutstandingAmount,
    int UnpaidPurchaseCount);
