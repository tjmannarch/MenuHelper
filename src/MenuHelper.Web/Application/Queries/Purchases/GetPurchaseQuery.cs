using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.PurchaseAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MenuHelper.Web.Application.Queries.Purchases;

public record GetPurchaseQuery(PurchaseId PurchaseId) : IQuery<PurchaseDetailDto>;

public class GetPurchaseQueryValidator : AbstractValidator<GetPurchaseQuery>
{
    public GetPurchaseQueryValidator()
    {
        RuleFor(x => x.PurchaseId).NotEmpty().WithMessage("进货记录ID不能为空");
    }
}

public class GetPurchaseQueryHandler(ApplicationDbContext context) : IQueryHandler<GetPurchaseQuery, PurchaseDetailDto>
{
    public async Task<PurchaseDetailDto> Handle(GetPurchaseQuery request, CancellationToken cancellationToken)
    {
        var purchase = await context.Set<Purchase>()
            .Include(p => p.Items)
            .Where(p => p.Id == request.PurchaseId && !p.Deleted)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KnownException($"未找到进货记录，Id = {request.PurchaseId}");

        return new PurchaseDetailDto(
            purchase.Id,
            purchase.SupplierId,
            purchase.SupplierName,
            purchase.PurchaseDate,
            purchase.IsPaid,
            purchase.Remark,
            purchase.Items.Sum(i => i.Quantity * i.UnitPrice),
            purchase.Items.Select(i => new PurchaseItemDto(
                i.Id, i.IngredientId, i.IngredientName, i.Unit, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice))
                .ToList());
    }
}

public record PurchaseItemDto(
    PurchaseItemId Id,
    IngredientId IngredientId,
    string IngredientName,
    string Unit,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalPrice);

public record PurchaseDetailDto(
    PurchaseId Id,
    SupplierId? SupplierId,
    string? SupplierName,
    DateOnly PurchaseDate,
    bool IsPaid,
    string? Remark,
    decimal TotalAmount,
    List<PurchaseItemDto> Items);
