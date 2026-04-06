using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace MenuHelper.Web.Application.Queries.Finance;

public record GetFinanceOverviewQuery(DateOnly From, DateOnly To) : IQuery<FinanceOverviewDto>;

public class GetFinanceOverviewQueryValidator : AbstractValidator<GetFinanceOverviewQuery>
{
    public GetFinanceOverviewQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From).WithMessage("结束日期不能早于开始日期");
    }
}

public class GetFinanceOverviewQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetFinanceOverviewQuery, FinanceOverviewDto>
{
    public async Task<FinanceOverviewDto> Handle(GetFinanceOverviewQuery q, CancellationToken ct)
    {
        var totalRevenue = await context.DailyRevenues
            .Where(x => x.Date >= q.From && x.Date <= q.To)
            .SumAsync(x => x.Amount, ct);

        var totalPurchaseCost = await context.Purchases
            .Where(p => !p.Deleted && p.PurchaseDate >= q.From && p.PurchaseDate <= q.To)
            .SelectMany(p => p.Items)
            .SumAsync(i => i.Quantity * i.UnitPrice, ct);

        return new FinanceOverviewDto(totalRevenue, totalPurchaseCost, totalRevenue - totalPurchaseCost);
    }
}

public record FinanceOverviewDto(decimal TotalRevenue, decimal TotalPurchaseCost, decimal GrossProfit);
