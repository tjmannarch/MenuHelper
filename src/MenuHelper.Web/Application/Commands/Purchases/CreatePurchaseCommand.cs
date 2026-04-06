using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.PurchaseAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MenuHelper.Web.Application.Commands.Purchases;

public record CreatePurchaseItemDto(IngredientId IngredientId, decimal Quantity, decimal UnitPrice);

public record CreatePurchaseCommand(
    SupplierId? SupplierId,
    DateOnly PurchaseDate,
    List<CreatePurchaseItemDto> Items,
    string? Remark) : ICommand<PurchaseId>;

public class CreatePurchaseCommandValidator : AbstractValidator<CreatePurchaseCommand>
{
    public CreatePurchaseCommandValidator()
    {
        RuleFor(x => x.PurchaseDate).NotEmpty().WithMessage("进货日期不能为空");
        RuleFor(x => x.Items).NotEmpty().WithMessage("进货明细不能为空");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.IngredientId).NotEmpty().WithMessage("原材料ID不能为空");
            item.RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("进货数量必须大于0");
            item.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("单价不能为负数");
        });
    }
}

public class CreatePurchaseCommandHandler(
    IPurchaseRepository purchaseRepository,
    ISupplierRepository supplierRepository,
    ApplicationDbContext context)
    : ICommandHandler<CreatePurchaseCommand, PurchaseId>
{
    public async Task<PurchaseId> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        string? supplierName = null;
        if (request.SupplierId != null)
        {
            var supplier = await supplierRepository.GetAsync(request.SupplierId, cancellationToken)
                ?? throw new KnownException($"未找到供应商，Id = {request.SupplierId}");
            if (supplier.Deleted)
                throw new KnownException("供应商已删除");
            supplierName = supplier.Name;
        }

        var ingredientIds = request.Items.Select(i => i.IngredientId).ToList();
        var ingredients = await context.Set<Ingredient>()
            .Where(i => ingredientIds.Contains(i.Id) && !i.Deleted)
            .Select(i => new { i.Id, i.Name, i.Unit })
            .ToListAsync(cancellationToken);

        var items = request.Items.Select(i =>
        {
            var ingredient = ingredients.FirstOrDefault(ing => ing.Id == i.IngredientId)
                ?? throw new KnownException($"未找到原材料，Id = {i.IngredientId}");
            return (i.IngredientId, ingredient.Name, ingredient.Unit, i.Quantity, i.UnitPrice);
        });

        var purchase = new Purchase(request.SupplierId, supplierName, request.PurchaseDate, items, request.Remark);
        await purchaseRepository.AddAsync(purchase, cancellationToken);
        return purchase.Id;
    }
}
