using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Domain.DomainEvents;

namespace MenuHelper.Domain.AggregatesModel.IngredientAggregate;

public partial record IngredientId : IGuidStronglyTypedId;

public enum IngredientCategory
{
    LiangPi = 1,      // 凉皮类
    RouJiaMo = 2,     // 肉夹馍类
    ShiGuoFan = 3,    // 石锅饭类
    Common = 4        // 通用食材
}

public enum ConsumptionType
{
    Immediate = 1,    // 即时消耗
    Amortized = 2     // 摊销消耗
}

public class Ingredient : Entity<IngredientId>, IAggregateRoot
{
    protected Ingredient() { }

    public Ingredient(string name, string unit, IngredientCategory category,
        ConsumptionType consumptionType, decimal defaultUnitPrice,
        decimal? safetyStockLevel = null, int? restockCycleDays = null,
        int? maxShelfDays = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new KnownException("原材料名称不能为空");
        if (string.IsNullOrWhiteSpace(unit)) throw new KnownException("计量单位不能为空");
        if (defaultUnitPrice < 0) throw new KnownException("默认单价不能小于0");
        if (safetyStockLevel.HasValue && safetyStockLevel < 0) throw new KnownException("安全库存线不能小于0");
        if (restockCycleDays.HasValue && restockCycleDays < 1) throw new KnownException("备货周期不能小于1天");
        if (maxShelfDays.HasValue && maxShelfDays < 1) throw new KnownException("最长存放天数不能小于1天");

        Name = name;
        Unit = unit;
        Category = category;
        ConsumptionType = consumptionType;
        DefaultUnitPrice = defaultUnitPrice;
        SafetyStockLevel = safetyStockLevel;
        RestockCycleDays = restockCycleDays;
        MaxShelfDays = maxShelfDays;
        this.AddDomainEvent(new IngredientCreatedDomainEvent(this));
    }

    public string Name { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public IngredientCategory Category { get; private set; }
    public ConsumptionType ConsumptionType { get; private set; }
    public SupplierId? SupplierId { get; private set; }
    public decimal? SafetyStockLevel { get; private set; }
    public int? RestockCycleDays { get; private set; }
    public int? MaxShelfDays { get; private set; }
    public decimal DefaultUnitPrice { get; private set; }
    public Deleted Deleted { get; private set; } = new();
    public RowVersion RowVersion { get; private set; } = new(0);

    public void UpdateBasicInfo(string name, string unit, IngredientCategory category,
        ConsumptionType consumptionType, decimal defaultUnitPrice,
        decimal? safetyStockLevel, int? restockCycleDays, int? maxShelfDays)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new KnownException("原材料名称不能为空");
        if (string.IsNullOrWhiteSpace(unit)) throw new KnownException("计量单位不能为空");
        if (defaultUnitPrice < 0) throw new KnownException("默认单价不能小于0");
        if (safetyStockLevel.HasValue && safetyStockLevel < 0) throw new KnownException("安全库存线不能小于0");
        if (restockCycleDays.HasValue && restockCycleDays < 1) throw new KnownException("备货周期不能小于1天");
        if (maxShelfDays.HasValue && maxShelfDays < 1) throw new KnownException("最长存放天数不能小于1天");

        Name = name;
        Unit = unit;
        Category = category;
        ConsumptionType = consumptionType;
        DefaultUnitPrice = defaultUnitPrice;
        SafetyStockLevel = safetyStockLevel;
        RestockCycleDays = restockCycleDays;
        MaxShelfDays = maxShelfDays;
        this.AddDomainEvent(new IngredientUpdatedDomainEvent(this));
    }

    public void BindSupplier(SupplierId? supplierId)
    {
        SupplierId = supplierId;
        this.AddDomainEvent(new IngredientSupplierBoundDomainEvent(this));
    }

    public void Delete()
    {
        Deleted = new Deleted(true);
    }
}
