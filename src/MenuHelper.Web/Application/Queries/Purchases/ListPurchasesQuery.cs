using MenuHelper.Domain.AggregatesModel.PurchaseAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MenuHelper.Web.Application.Queries.Purchases;

public record ListPurchasesQuery(
    SupplierId? SupplierId = null,
    DateOnly? From = null,
    DateOnly? To = null,
    bool? IsPaid = null,
    int PageIndex = 1,
    int PageSize = 20,
    bool CountTotal = true) : IPagedQuery<PurchaseListItemDto>;

public class ListPurchasesQueryValidator : AbstractValidator<ListPurchasesQuery>
{
    public ListPurchasesQueryValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(1).WithMessage("页码必须大于或等于1");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("每页条数必须在1到100之间");
    }
}

public class ListPurchasesQueryHandler(ApplicationDbContext context)
    : IQueryHandler<ListPurchasesQuery, PagedData<PurchaseListItemDto>>
{
    public async Task<PagedData<PurchaseListItemDto>> Handle(ListPurchasesQuery request, CancellationToken cancellationToken)
    {
        return await context.Set<Purchase>()
            .Where(p => !p.Deleted)
            .WhereIf(request.SupplierId != null, p => p.SupplierId == request.SupplierId)
            .WhereIf(request.From.HasValue, p => p.PurchaseDate >= request.From!.Value)
            .WhereIf(request.To.HasValue, p => p.PurchaseDate <= request.To!.Value)
            .WhereIf(request.IsPaid.HasValue, p => p.IsPaid == request.IsPaid!.Value)
            .OrderByDescending(p => p.PurchaseDate)
            .ThenByDescending(p => p.Id)
            .Select(p => new PurchaseListItemDto(
                p.Id,
                p.SupplierId,
                p.SupplierName,
                p.PurchaseDate,
                p.IsPaid,
                p.Items.Sum(i => i.Quantity * i.UnitPrice),
                p.Items.Count))
            .ToPagedDataAsync(request, cancellationToken: cancellationToken);
    }
}

public record PurchaseListItemDto(
    PurchaseId Id,
    SupplierId? SupplierId,
    string? SupplierName,
    DateOnly PurchaseDate,
    bool IsPaid,
    decimal TotalAmount,
    int ItemCount);
