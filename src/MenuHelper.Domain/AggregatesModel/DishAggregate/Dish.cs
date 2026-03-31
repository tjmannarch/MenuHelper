using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.DomainEvents;

namespace MenuHelper.Domain.AggregatesModel.DishAggregate;

public partial record DishId : IGuidStronglyTypedId;
public partial record DishIngredientId : IGuidStronglyTypedId;

public enum QuantityType
{
    Fixed = 1,      // 固定用量
    Variable = 2    // 非固定用量
}

public class Dish : Entity<DishId>, IAggregateRoot
{
    protected Dish() { }

    public Dish(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new KnownException("菜品名称不能为空");
        Name = name;
        this.AddDomainEvent(new DishCreatedDomainEvent(this));
    }

    public string Name { get; private set; } = string.Empty;
    private List<DishIngredient> _dishIngredients = [];
    public IReadOnlyList<DishIngredient> DishIngredients => _dishIngredients.AsReadOnly();
    public Deleted Deleted { get; private set; } = new();
    public RowVersion RowVersion { get; private set; } = new(0);

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new KnownException("菜品名称不能为空");
        Name = name;
    }

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

    public void UpdateIngredient(IngredientId ingredientId, QuantityType quantityType, decimal? fixedQuantity)
    {
        var item = _dishIngredients.FirstOrDefault(x => x.IngredientId == ingredientId)
            ?? throw new KnownException("该原材料未关联到此菜品");
        if (quantityType == QuantityType.Fixed && (!fixedQuantity.HasValue || fixedQuantity <= 0))
            throw new KnownException("固定用量必须大于0");
        item.Update(quantityType, fixedQuantity);
    }

    public void RemoveIngredient(IngredientId ingredientId)
    {
        var item = _dishIngredients.FirstOrDefault(x => x.IngredientId == ingredientId)
            ?? throw new KnownException("该原材料未关联到此菜品");
        _dishIngredients.Remove(item);
        this.AddDomainEvent(new DishIngredientRemovedDomainEvent(this, ingredientId));
    }

    public void Delete()
    {
        Deleted = new Deleted(true);
    }
}

public class DishIngredient : Entity<DishIngredientId>
{
    protected DishIngredient() { }

    public DishIngredient(IngredientId ingredientId, QuantityType quantityType, decimal? fixedQuantity)
    {
        IngredientId = ingredientId;
        QuantityType = quantityType;
        FixedQuantity = fixedQuantity;
    }

    public IngredientId IngredientId { get; private set; } = default!;
    public QuantityType QuantityType { get; private set; }
    public decimal? FixedQuantity { get; private set; }

    public void Update(QuantityType quantityType, decimal? fixedQuantity)
    {
        QuantityType = quantityType;
        FixedQuantity = fixedQuantity;
    }
}
