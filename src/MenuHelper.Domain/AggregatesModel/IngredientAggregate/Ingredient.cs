using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Domain.DomainEvents;

namespace MenuHelper.Domain.AggregatesModel.IngredientAggregate;

public partial record IngredientId : IGuidStronglyTypedId;

/// <summary>菜品分类（用于采购统计分组）</summary>
public enum IngredientCategory
{
    /// <summary>凉皮类</summary>
    LiangPi = 1,
    /// <summary>肉夹馍类</summary>
    RouJiaMo = 2,
    /// <summary>石锅饭类</summary>
    ShiGuoFan = 3,
    /// <summary>通用食材</summary>
    Common = 4
}

/// <summary>消耗方式（用于成本计算）</summary>
public enum ConsumptionType
{
    /// <summary>即时消耗：当日采购全额计入成本</summary>
    Immediate = 1,
    /// <summary>摊销消耗：按历史进货周期每日均摊成本</summary>
    Amortized = 2
}

/// <summary>
/// 原材料聚合
/// </summary>
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

    /// <summary>原材料名称</summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>计量单位（斤/个/包等）</summary>
    public string Unit { get; private set; } = string.Empty;
    /// <summary>菜品分类（用于采购统计分组）</summary>
    public IngredientCategory Category { get; private set; }
    /// <summary>消耗方式（用于成本计算）</summary>
    public ConsumptionType ConsumptionType { get; private set; }
    /// <summary>供应商标识，null 表示自购</summary>
    public SupplierId? SupplierId { get; private set; }
    /// <summary>安全库存线，低于此值触发补货提醒</summary>
    public decimal? SafetyStockLevel { get; private set; }
    /// <summary>默认备货周期（天），超过此周期未进货触发提醒</summary>
    public int? RestockCycleDays { get; private set; }
    /// <summary>最长存放天数，超期触发新鲜度预警</summary>
    public int? MaxShelfDays { get; private set; }
    /// <summary>默认单价（预设基准值，仅影响未来采购，不改历史账单）</summary>
    public decimal DefaultUnitPrice { get; private set; }
    /// <summary>软删除标记</summary>
    public Deleted Deleted { get; private set; } = new();
    /// <summary>乐观并发控制版本号</summary>
    public RowVersion RowVersion { get; private set; } = new(0);

    /// <summary>
    /// 更新原材料基本信息，包括名称、单位、分类、消耗方式、默认单价及库存参数
    /// </summary>
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

    /// <summary>
    /// 绑定或更换供应商；传入 null 表示标记为自购
    /// </summary>
    public void BindSupplier(SupplierId? supplierId)
    {
        SupplierId = supplierId;
        this.AddDomainEvent(new IngredientSupplierBoundDomainEvent(this));
    }

    /// <summary>
    /// 软删除原材料（将 Deleted 置为 true）
    /// </summary>
    public void Delete()
    {
        Deleted = new Deleted(true);
    }
}
