using MenuHelper.Domain.AggregatesModel.SupplierAggregate;

namespace MenuHelper.Infrastructure.Repositories;

public interface ISupplierRepository : IRepository<Supplier, SupplierId>
{
}

public class SupplierRepository(ApplicationDbContext context)
    : RepositoryBase<Supplier, SupplierId, ApplicationDbContext>(context), ISupplierRepository
{
}
