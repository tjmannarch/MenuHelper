# 菜单助手 - CleanDDD 建模文档

> 每个需求模块的建模蓝图（聚合/命令/查询/事件/端点），按模块追加。

---

## 模块一：菜品与原材料管理（REQ-001~009）

### 聚合

#### 1. Ingredient（原材料）

**职责**：维护原材料的基本信息、分类、消耗方式、库存参数、供应商绑定与默认单价

**关键不变式**：
- 名称不能为空
- 默认单价 ≥ 0
- 安全库存线 ≥ 0（若设置）
- 备货周期 ≥ 1 天（若设置）
- 最长存放天数 ≥ 1 天（若设置）

| 属性 | 类型 | 说明 |
|------|------|------|
| Id | IngredientId（Guid） | 强类型ID |
| Name | string | 原材料名称 |
| Unit | string | 计量单位（斤/个/包等） |
| Category | IngredientCategory（枚举） | 凉皮类/肉夹馍类/石锅饭类/通用食材 |
| ConsumptionType | ConsumptionType（枚举） | 按库存盘点/按周期摊销/按销量推算 |
| SupplierId | SupplierId?（可空） | null = 自购 |
| SafetyStockLevel | decimal? | 安全库存线 |
| RestockCycleDays | int? | 默认备货周期（天） |
| MaxShelfDays | int? | 最长存放天数 |
| DefaultUnitPrice | decimal | 默认单价（预设基准值） |

**行为 → 触发领域事件**：

| 行为 | 触发事件 |
|------|---------|
| 构造（Create） | IngredientCreatedDomainEvent |
| UpdateBasicInfo | IngredientUpdatedDomainEvent |
| BindSupplier | IngredientSupplierBoundDomainEvent |
| UnbindSupplier（标记自购） | IngredientSupplierBoundDomainEvent |

---

#### 2. Dish（菜品）

**职责**：维护菜品信息及与原材料的关联关系（含用量类型）

**关键不变式**：
- 名称不能为空
- 同一菜品不可重复关联同一原材料

| 属性 | 类型 | 说明 |
|------|------|------|
| Id | DishId（Guid） | 强类型ID |
| Name | string | 菜品名称 |
| DishIngredients | List\<DishIngredient\>（子实体） | 关联原材料列表 |

**DishIngredient 子实体**：

| 属性 | 类型 | 说明 |
|------|------|------|
| Id | DishIngredientId（Guid） | 强类型ID |
| IngredientId | IngredientId | 仅存 ID，不持有引用 |
| QuantityType | QuantityType（枚举） | 固定/非固定 |
| FixedQuantity | decimal? | QuantityType=固定时填写 |

**行为 → 触发领域事件**：

| 行为 | 触发事件 |
|------|---------|
| 构造（Create） | DishCreatedDomainEvent |
| AddIngredient | DishIngredientAddedDomainEvent |
| UpdateIngredient | — |
| RemoveIngredient | DishIngredientRemovedDomainEvent |

---

### 命令（Commands）

| 命令 | 作用聚合 | 输入 | 触发行为/事件 | 幂等 |
|------|---------|------|------------|------|
| CreateIngredientCommand | Ingredient | Name, Unit, Category, ConsumptionType, SupplierId?, SafetyStockLevel?, RestockCycleDays?, MaxShelfDays?, DefaultUnitPrice | Create → IngredientCreatedDomainEvent | 否 |
| UpdateIngredientCommand | Ingredient | IngredientId, Name, Unit, Category, ConsumptionType, SafetyStockLevel?, RestockCycleDays?, MaxShelfDays?, DefaultUnitPrice | UpdateBasicInfo → IngredientUpdatedDomainEvent | 是 |
| BindIngredientSupplierCommand | Ingredient | IngredientId, SupplierId?（null=自购） | BindSupplier/UnbindSupplier → IngredientSupplierBoundDomainEvent | 是 |
| DeleteIngredientCommand | Ingredient | IngredientId | 软删除 | 是 |
| CreateDishCommand | Dish | Name | Create → DishCreatedDomainEvent | 否 |
| UpdateDishCommand | Dish | DishId, Name | UpdateName | 是 |
| DeleteDishCommand | Dish | DishId | 软删除 | 是 |
| AddDishIngredientCommand | Dish | DishId, IngredientId, QuantityType, FixedQuantity? | AddIngredient → DishIngredientAddedDomainEvent | 否 |
| UpdateDishIngredientCommand | Dish | DishId, IngredientId, QuantityType, FixedQuantity? | UpdateIngredient | 是 |
| RemoveDishIngredientCommand | Dish | DishId, IngredientId | RemoveIngredient → DishIngredientRemovedDomainEvent | 是 |

---

### 查询（Queries）

| 查询 | 过滤/排序 | 输出 |
|------|---------|------|
| GetIngredientQuery | IngredientId | IngredientDetailDto（全字段） |
| ListIngredientsQuery | Category?, ConsumptionType?, keyword?, 分页 | PagedData\<IngredientListItemDto\> |
| GetDishQuery | DishId | DishDetailDto（含 DishIngredient 列表） |
| ListDishesQuery | keyword?, 分页 | PagedData\<DishListItemDto\> |

---

### 领域事件

| 事件 | 携带数据 | 用途 |
|------|---------|------|
| IngredientCreatedDomainEvent | Ingredient | 预留，后续模块订阅 |
| IngredientUpdatedDomainEvent | Ingredient | 预留，后续模块订阅 |
| IngredientSupplierBoundDomainEvent | Ingredient | 预留，后续模块订阅 |
| DishCreatedDomainEvent | Dish | 预留 |
| DishIngredientAddedDomainEvent | Dish, IngredientId | 预留 |
| DishIngredientRemovedDomainEvent | Dish, IngredientId | 预留 |

---

### API 端点（Endpoints）

> 当前所有端点使用 `[AllowAnonymous]`（单用户系统）

| 方法 & 路径 | 绑定命令/查询 | 说明 |
|------------|------------|------|
| POST `/api/ingredients` | CreateIngredientCommand | 创建原材料 |
| PUT `/api/ingredients/{id}` | UpdateIngredientCommand | 修改原材料 |
| PUT `/api/ingredients/{id}/supplier` | BindIngredientSupplierCommand | 绑定/解绑供应商 |
| DELETE `/api/ingredients/{id}` | DeleteIngredientCommand | 删除原材料 |
| GET `/api/ingredients/{id}` | GetIngredientQuery | 查询单个原材料 |
| GET `/api/ingredients` | ListIngredientsQuery | 分页查询原材料列表 |
| POST `/api/dishes` | CreateDishCommand | 创建菜品 |
| PUT `/api/dishes/{id}` | UpdateDishCommand | 修改菜品名称 |
| DELETE `/api/dishes/{id}` | DeleteDishCommand | 删除菜品 |
| GET `/api/dishes/{id}` | GetDishQuery | 查询单个菜品（含原材料关联） |
| GET `/api/dishes` | ListDishesQuery | 分页查询菜品列表 |
| POST `/api/dishes/{id}/ingredients` | AddDishIngredientCommand | 添加菜品原材料关联 |
| PUT `/api/dishes/{id}/ingredients/{ingredientId}` | UpdateDishIngredientCommand | 修改关联用量 |
| DELETE `/api/dishes/{id}/ingredients/{ingredientId}` | RemoveDishIngredientCommand | 移除关联 |

---

### 业务规则补充

- **DeleteIngredientCommand**：若该原材料已被菜品关联，拒绝删除，提示"请先移除相关菜品关联"
- **AddDishIngredientCommand**：QuantityType=固定时，FixedQuantity 必须 > 0

---
