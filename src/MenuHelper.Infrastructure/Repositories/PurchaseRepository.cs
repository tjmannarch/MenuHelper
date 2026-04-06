using MenuHelper.Domain.AggregatesModel.PurchaseAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;

namespace MenuHelper.Infrastructure.Repositories;

public interface IPurchaseRepository : IRepository<Purchase, PurchaseId>
{
    Task<List<Purchase>> GetUnpaidBySupplierAsync(SupplierId supplierId, CancellationToken cancellationToken = default);
}

public class PurchaseRepository(ApplicationDbContext context)
    : RepositoryBase<Purchase, PurchaseId, ApplicationDbContext>(context), IPurchaseRepository
{
    public async Task<List<Purchase>> GetUnpaidBySupplierAsync(SupplierId supplierId, CancellationToken cancellationToken = default)
        => await DbContext.Set<Purchase>()
            .Where(p => p.SupplierId == supplierId && !p.IsPaid && !p.Deleted)
            .ToListAsync(cancellationToken);
}
