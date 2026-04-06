using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MenuHelper.Web.Application.Queries.Suppliers;

public record ListSuppliersQuery(
    string? Keyword = null,
    int PageIndex = 1,
    int PageSize = 20,
    bool CountTotal = true) : IPagedQuery<SupplierListItemDto>;

public class ListSuppliersQueryValidator : AbstractValidator<ListSuppliersQuery>
{
    public ListSuppliersQueryValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(1).WithMessage("页码必须大于或等于1");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("每页条数必须在1到100之间");
    }
}

public class ListSuppliersQueryHandler(ApplicationDbContext context)
    : IQueryHandler<ListSuppliersQuery, PagedData<SupplierListItemDto>>
{
    public async Task<PagedData<SupplierListItemDto>> Handle(ListSuppliersQuery request, CancellationToken cancellationToken)
    {
        return await context.Set<Supplier>()
            .Where(x => !x.Deleted)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Keyword), x => x.Name.Contains(request.Keyword!))
            .OrderBy(x => x.Name)
            .Select(x => new SupplierListItemDto(x.Id, x.Name, x.Phone, x.Remark))
            .ToPagedDataAsync(request, cancellationToken: cancellationToken);
    }
}

public record SupplierListItemDto(SupplierId Id, string Name, string? Phone, string? Remark);
