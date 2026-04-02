using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.DomainEvents;

namespace MenuHelper.Domain.AggregatesModel.DishAggregate;

public partial record DishId : IGuidStronglyTypedId;
public partial record DishIngredientId : IGuidStronglyTypedId;

/// <summary>用量类型</summary>
public enum QuantityType
{
    /// <summary>固定用量（如 1份肉夹馍 = 1个白饼）</summary>
    Fixed = 1,
    /// <summary>非固定用量（如腊汁肉，每份手工分量不固定）</summary>
    Variable = 2
}

/// <summary>
/// 菜品聚合
/// </summary>
public class Dish : Entity<DishId>, IAggregateRoot
{
    protected Dish() { }

    public Dish(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new KnownException("菜品名称不能为空");
        Name = name;
        this.AddDomainEvent(new DishCreatedDomainEvent(this));
    }

    /// <summary>菜品名称</summary>
    public string Name { get; private set; } = string.Empty;
    private readonly List<DishIngredient> _dishIngredients = [];
    /// <summary>菜品关联的原材料列表</summary>
    public IReadOnlyList<DishIngredient> DishIngredients => _dishIngredients.AsReadOnly();
    /// <summary>软删除标记</summary>
    public Deleted Deleted { get; private set; } = new();
    /// <summary>乐观并发控制版本号</summary>
    public RowVersion RowVersion { get; private set; } = new(0);

    /// <summary>
    /// 修改菜品名称
    /// </summary>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new KnownException("菜品名称不能为空");
        Name = name;
    }

    /// <summary>
    /// 添加原材料关联；同一原材料不可重复添加，固定用量时 fixedQuantity 必须大于 0
    /// </summary>
    public void AddIngredient(IngredientId ingredientId, QuantityType quantityType, decimal? fixedQuantity)
    {
        if (_dishIngredients.Any(x => x.IngredientId == ingredientId))
            throw new KnownException("该菜品已关联此原材料");
        if (quantityType == QuantityType.Fixed && (!fixedQuantity.HasValue || fixedQuantity <= 0))
            throw new KnownException("固定用量必须大于0");

        var item = new DishIngredient(ingredientId, quantityType, fixedQuantity);
        _dishIngredients.Add(item);
        this.AddDomainEvent(new DishIngredientAddedDomainEvent(this, ingredientId));
    }

    /// <summary>
    /// 修改已关联原材料的用量类型和固定用量
    /// </summary>
    public void UpdateIngredient(IngredientId ingredientId, QuantityType quantityType, decimal? fixedQuantity)
    {
        var item = _dishIngredients.FirstOrDefault(x => x.IngredientId == ingredientId)
            ?? throw new KnownException("该原材料未关联到此菜品");
        if (quantityType == QuantityType.Fixed && (!fixedQuantity.HasValue || fixedQuantity <= 0))
            throw new KnownException("固定用量必须大于0");
        item.Update(quantityType, fixedQuantity);
    }

    /// <summary>
    /// 移除原材料关联
    /// </summary>
    public void RemoveIngredient(IngredientId ingredientId)
    {
        var item = _dishIngredients.FirstOrDefault(x => x.IngredientId == ingredientId)
            ?? throw new KnownException("该原材料未关联到此菜品");
        _dishIngredients.Remove(item);
        this.AddDomainEvent(new DishIngredientRemovedDomainEvent(this, ingredientId));
    }

    /// <summary>
    /// 软删除菜品（将 Deleted 置为 true）
    /// </summary>
    public void Delete()
    {
        Deleted = new Deleted(true);
    }
}

/// <summary>
/// 菜品原材料
/// </summary>
public class DishIngredient : Entity<DishIngredientId>
{
    protected DishIngredient() { }

    public DishIngredient(IngredientId ingredientId, QuantityType quantityType, decimal? fixedQuantity)
    {
        IngredientId = ingredientId;
        QuantityType = quantityType;
        FixedQuantity = fixedQuantity;
    }

    /// <summary>原材料标识</summary>
    public IngredientId IngredientId { get; private set; } = default!;
    /// <summary>用量类型（固定/非固定）</summary>
    public QuantityType QuantityType { get; private set; }
    /// <summary>固定用量数值，QuantityType = Fixed 时有效</summary>
    public decimal? FixedQuantity { get; private set; }

    /// <summary>
    /// 修改用量类型和固定用量
    /// </summary>
    public void Update(QuantityType quantityType, decimal? fixedQuantity)
    {
        QuantityType = quantityType;
        FixedQuantity = fixedQuantity;
    }
}
