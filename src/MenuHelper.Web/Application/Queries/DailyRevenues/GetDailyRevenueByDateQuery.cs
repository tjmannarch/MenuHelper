using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace MenuHelper.Web.Application.Queries.DailyRevenues;

public record GetDailyRevenueByDateQuery(DateOnly Date) : IQuery<DailyRevenueDto?>;

public class GetDailyRevenueByDateQueryValidator : AbstractValidator<GetDailyRevenueByDateQuery> { }

public class GetDailyRevenueByDateQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetDailyRevenueByDateQuery, DailyRevenueDto?>
{
    public async Task<DailyRevenueDto?> Handle(GetDailyRevenueByDateQuery q, CancellationToken ct)
        => await context.DailyRevenues
            .Where(x => x.Date == q.Date)
            .Select(x => new DailyRevenueDto(x.Date.ToString("yyyy-MM-dd"), x.Amount, x.Note))
            .FirstOrDefaultAsync(ct);
}

public record DailyRevenueDto(string Date, decimal Amount, string? Note);
