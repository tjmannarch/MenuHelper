using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MenuHelper.Web.Application.Queries.PurchaseOrders;

public record PurchaseOrderItemRequest(IngredientId IngredientId, decimal Quantity);

public record GeneratePurchaseOrderQuery(
    List<PurchaseOrderItemRequest> Items,
    DateOnly? OrderDate = null) : IQuery<PurchaseOrderDto>;

public class GeneratePurchaseOrderQueryValidator : AbstractValidator<GeneratePurchaseOrderQuery>
{
    public GeneratePurchaseOrderQueryValidator()
    {
        RuleFor(x => x.Items).NotEmpty().WithMessage("采购清单不能为空");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.IngredientId).NotEmpty().WithMessage("原材料ID不能为空");
            item.RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("采购数量必须大于0");
        });
    }
}

public class GeneratePurchaseOrderQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GeneratePurchaseOrderQuery, PurchaseOrderDto>
{
    public async Task<PurchaseOrderDto> Handle(GeneratePurchaseOrderQuery request, CancellationToken cancellationToken)
    {
        var ingredientIds = request.Items.Select(i => i.IngredientId).ToList();
        var ingredients = await context.Set<Ingredient>()
            .Where(i => ingredientIds.Contains(i.Id) && !i.Deleted)
            .Select(i => new { i.Id, i.Name, i.Unit, i.SupplierId, i.DefaultUnitPrice })
            .ToListAsync(cancellationToken);

        var supplierIds = ingredients
            .Where(i => i.SupplierId != null)
            .Select(i => i.SupplierId!)
            .Distinct()
            .ToList();

        var suppliers = await context.Set<Supplier>()
            .Where(s => supplierIds.Contains(s.Id) && !s.Deleted)
            .Select(s => new { s.Id, s.Name, s.Phone })
            .ToListAsync(cancellationToken);

        var orderDate = request.OrderDate ?? DateOnly.FromDateTime(DateTime.Today);

        var supplierGroups = new List<SupplierOrderDto>();
        var selfPurchaseItems = new List<PurchaseOrderLineDto>();

        var groupedBySupplier = request.Items
            .Select(reqItem =>
            {
                var ingredient = ingredients.FirstOrDefault(i => i.Id == reqItem.IngredientId);
                return new
                {
                    reqItem.IngredientId,
                    reqItem.Quantity,
                    IngredientName = ingredient?.Name ?? "未知原材料",
                    Unit = ingredient?.Unit ?? "",
                    SupplierId = ingredient?.SupplierId,
                    DefaultUnitPrice = ingredient?.DefaultUnitPrice
                };
            })
            .GroupBy(x => x.SupplierId);

        foreach (var group in groupedBySupplier)
        {
            var lines = group.Select(x => new PurchaseOrderLineDto(
                x.IngredientId, x.IngredientName, x.Unit, x.Quantity, x.DefaultUnitPrice)).ToList();

            if (group.Key == null)
            {
                selfPurchaseItems.AddRange(lines);
            }
            else
            {
                var supplier = suppliers.FirstOrDefault(s => s.Id == group.Key);
                var supplierName = supplier?.Name ?? "未知供应商";
                var phone = supplier?.Phone;

                var text = BuildOrderText(supplierName, phone, orderDate, lines);
                supplierGroups.Add(new SupplierOrderDto(group.Key, supplierName, phone, lines, text));
            }
        }

        string? selfPurchaseText = selfPurchaseItems.Count > 0
            ? BuildSelfPurchaseText(orderDate, selfPurchaseItems)
            : null;

        return new PurchaseOrderDto(orderDate, supplierGroups, selfPurchaseItems, selfPurchaseText);
    }

    private static string BuildOrderText(string supplierName, string? phone, DateOnly date, List<PurchaseOrderLineDto> lines)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"【采购单】{date:yyyy年MM月dd日}");
        sb.AppendLine($"供应商：{supplierName}");
        if (!string.IsNullOrWhiteSpace(phone))
            sb.AppendLine($"电话：{phone}");
        sb.AppendLine("---");
        foreach (var line in lines)
        {
            var priceStr = line.EstimatedUnitPrice.HasValue ? $" × {line.EstimatedUnitPrice:F2}元/{line.Unit}" : "";
            sb.AppendLine($"{line.IngredientName}：{line.Quantity}{line.Unit}{priceStr}");
        }
        return sb.ToString().TrimEnd();
    }

    private static string BuildSelfPurchaseText(DateOnly date, List<PurchaseOrderLineDto> lines)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"【自购清单】{date:yyyy年MM月dd日}");
        sb.AppendLine("---");
        foreach (var line in lines)
            sb.AppendLine($"{line.IngredientName}：{line.Quantity}{line.Unit}");
        return sb.ToString().TrimEnd();
    }
}

public record PurchaseOrderLineDto(
    IngredientId IngredientId,
    string IngredientName,
    string Unit,
    decimal Quantity,
    decimal? EstimatedUnitPrice);

public record SupplierOrderDto(
    SupplierId? SupplierId,
    string SupplierName,
    string? Phone,
    List<PurchaseOrderLineDto> Items,
    string OrderText);

public record PurchaseOrderDto(
    DateOnly OrderDate,
    List<SupplierOrderDto> SupplierOrders,
    List<PurchaseOrderLineDto> SelfPurchaseItems,
    string? SelfPurchaseText);
