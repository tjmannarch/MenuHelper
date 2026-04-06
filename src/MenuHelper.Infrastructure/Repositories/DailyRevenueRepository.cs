using MenuHelper.Domain.AggregatesModel.DailyRevenueAggregate;
namespace MenuHelper.Infrastructure.Repositories;

public interface IDailyRevenueRepository : IRepository<DailyRevenue, DailyRevenueId>
{
    Task<DailyRevenue?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);
}

public class DailyRevenueRepository(ApplicationDbContext context)
    : RepositoryBase<DailyRevenue, DailyRevenueId, ApplicationDbContext>(context), IDailyRevenueRepository
{
    public async Task<DailyRevenue?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
        => await DbContext.DailyRevenues.FirstOrDefaultAsync(x => x.Date == date && !x.Deleted, cancellationToken);
}
