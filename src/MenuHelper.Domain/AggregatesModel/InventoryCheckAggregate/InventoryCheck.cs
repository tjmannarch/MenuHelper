using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.DomainEvents;

namespace MenuHelper.Domain.AggregatesModel.InventoryCheckAggregate;

public partial record InventoryCheckId : IGuidStronglyTypedId;
public partial record InventoryCheckItemId : IGuidStronglyTypedId;

/// <summary>
/// 每日库存盘点聚合根。
/// 同一天允许多次修改，以最后一次为准。
/// </summary>
public class InventoryCheck : Entity<InventoryCheckId>, IAggregateRoot
{
    protected InventoryCheck() { }

    /// <summary>
    /// 创建一条新的库存盘点记录
    /// </summary>
    /// <param name="checkDate">盘点日期</param>
    /// <param name="items">原材料剩余库存明细，每种原材料仅可出现一次</param>
    public InventoryCheck(DateOnly checkDate, IEnumerable<(IngredientId IngredientId, decimal Quantity)> items)
    {
        CheckDate = checkDate;
        var itemList = items.ToList();
        if (itemList.Select(x => x.IngredientId).Distinct().Count() != itemList.Count)
            throw new KnownException("同一盘点中不允许重复录入同一原材料");
        foreach (var (ingredientId, quantity) in itemList)
        {
            if (quantity < 0) throw new KnownException("库存数量不能小于0");
            _items.Add(new InventoryCheckItem(ingredientId, quantity));
        }
        this.AddDomainEvent(new InventoryCheckCreatedDomainEvent(this));
    }

    /// <summary>盘点日期</summary>
    public DateOnly CheckDate { get; private set; }

    private readonly List<InventoryCheckItem> _items = [];

    /// <summary>各原材料剩余库存明细</summary>
    public IReadOnlyList<InventoryCheckItem> Items => _items.AsReadOnly();

    /// <summary>软删除标记</summary>
    public Deleted Deleted { get; private set; } = new();

    /// <summary>乐观并发控制版本号</summary>
    public RowVersion RowVersion { get; private set; } = new(0);

    /// <summary>
    /// 整体替换盘点明细（同一天允许多次修改，以最后一次为准）
    /// </summary>
    /// <param name="items">最新的原材料剩余库存明细，每种原材料仅可出现一次</param>
    public void SetItems(IEnumerable<(IngredientId IngredientId, decimal Quantity)> items)
    {
        var itemList = items.ToList();
        if (itemList.Select(x => x.IngredientId).Distinct().Count() != itemList.Count)
            throw new KnownException("同一盘点中不允许重复录入同一原材料");
        foreach (var (_, quantity) in itemList)
        {
            if (quantity < 0) throw new KnownException("库存数量不能小于0");
        }

        _items.Clear();
        foreach (var (ingredientId, quantity) in itemList)
            _items.Add(new InventoryCheckItem(ingredientId, quantity));

        this.AddDomainEvent(new InventoryCheckItemsUpdatedDomainEvent(this));
    }
}

/// <summary>
/// 库存盘点明细（子实体）
/// </summary>
public class InventoryCheckItem : Entity<InventoryCheckItemId>
{
    protected InventoryCheckItem() { }

    public InventoryCheckItem(IngredientId ingredientId, decimal quantity)
    {
        IngredientId = ingredientId;
        Quantity = quantity;
    }

    /// <summary>原材料标识</summary>
    public IngredientId IngredientId { get; private set; } = default!;

    /// <summary>剩余库存数量</summary>
    public decimal Quantity { get; private set; }
}
