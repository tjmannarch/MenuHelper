using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Domain.DomainEvents;

namespace MenuHelper.Domain.AggregatesModel.PurchaseAggregate;

public partial record PurchaseId : IGuidStronglyTypedId;
public partial record PurchaseItemId : IGuidStronglyTypedId;

public class Purchase : Entity<PurchaseId>, IAggregateRoot
{
    protected Purchase() { }

    public Purchase(
        SupplierId? supplierId,
        string? supplierName,
        DateOnly purchaseDate,
        IEnumerable<(IngredientId IngredientId, string IngredientName, string Unit, decimal Quantity, decimal UnitPrice)> items,
        string? remark)
    {
        SupplierId = supplierId;
        SupplierName = supplierName;
        PurchaseDate = purchaseDate;
        Remark = remark;

        var itemList = items.ToList();
        if (itemList.Count == 0)
            throw new KnownException("进货记录至少需要一条明细");

        foreach (var (ingredientId, ingredientName, unit, quantity, unitPrice) in itemList)
        {
            if (quantity <= 0) throw new KnownException("进货数量必须大于0");
            if (unitPrice < 0) throw new KnownException("单价不能为负数");
            _items.Add(new PurchaseItem(ingredientId, ingredientName, unit, quantity, unitPrice));
        }

        this.AddDomainEvent(new PurchaseCreatedDomainEvent(this));
    }

    public SupplierId? SupplierId { get; private set; }
    public string? SupplierName { get; private set; }
    public DateOnly PurchaseDate { get; private set; }
    public bool IsPaid { get; private set; }
    public string? Remark { get; private set; }

    private readonly List<PurchaseItem> _items = [];
    public IReadOnlyList<PurchaseItem> Items => _items.AsReadOnly();

    public Deleted Deleted { get; private set; } = new();
    public RowVersion RowVersion { get; private set; } = new(0);

    public void Pay()
    {
        if (IsPaid)
            throw new KnownException("该进货记录已结算");
        IsPaid = true;
        this.AddDomainEvent(new PurchasePaidDomainEvent(this));
    }
}

public class PurchaseItem : Entity<PurchaseItemId>
{
    protected PurchaseItem() { }

    public PurchaseItem(IngredientId ingredientId, string ingredientName, string unit, decimal quantity, decimal unitPrice)
    {
        IngredientId = ingredientId;
        IngredientName = ingredientName;
        Unit = unit;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public IngredientId IngredientId { get; private set; } = default!;
    public string IngredientName { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
}
