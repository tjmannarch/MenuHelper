using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace MenuHelper.Web.Application.Queries.DailyRevenues;

public record ListDailyRevenuesQuery(DateOnly From, DateOnly To) : IQuery<List<DailyRevenueDto>>;

public class ListDailyRevenuesQueryValidator : AbstractValidator<ListDailyRevenuesQuery>
{
    public ListDailyRevenuesQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From).WithMessage("结束日期必须大于等于开始日期");
    }
}

public class ListDailyRevenuesQueryHandler(ApplicationDbContext context)
    : IQueryHandler<ListDailyRevenuesQuery, List<DailyRevenueDto>>
{
    public async Task<List<DailyRevenueDto>> Handle(ListDailyRevenuesQuery q, CancellationToken ct)
        => await context.DailyRevenues
            .Where(x => x.Date >= q.From && x.Date <= q.To)
            .OrderByDescending(x => x.Date)
            .Select(x => new DailyRevenueDto(x.Date.ToString("yyyy-MM-dd"), x.Amount, x.Note))
            .ToListAsync(ct);
}
