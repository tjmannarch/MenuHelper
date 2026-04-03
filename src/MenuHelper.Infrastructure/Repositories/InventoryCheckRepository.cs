using MenuHelper.Domain.AggregatesModel.InventoryCheckAggregate;

namespace MenuHelper.Infrastructure.Repositories;

public interface IInventoryCheckRepository : IRepository<InventoryCheck, InventoryCheckId>
{
    /// <summary>
    /// 根据盘点日期获取当日盘点记录（含明细）
    /// </summary>
    Task<InventoryCheck?> GetByDateAsync(DateOnly checkDate, CancellationToken cancellationToken = default);
}

public class InventoryCheckRepository(ApplicationDbContext context)
    : RepositoryBase<InventoryCheck, InventoryCheckId, ApplicationDbContext>(context), IInventoryCheckRepository
{
    public async Task<InventoryCheck?> GetByDateAsync(DateOnly checkDate, CancellationToken cancellationToken = default)
    {
        return await DbContext.InventoryChecks
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.CheckDate == checkDate, cancellationToken);
    }
}
