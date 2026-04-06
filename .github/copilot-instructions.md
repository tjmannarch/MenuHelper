你的任务是按照 *.instructions.md 描述的规范完成功能开发任务。

## 工作流约定

- **收到"了解项目现状"/"读取检查点"/"分析"等指令时，只做信息摘要，不写任何代码、不创建文件、不创建 todos**，等待明确的开发指令（如"开始开发"/"完成这个任务"）
- 开始实现前，先列出任务计划，等用户确认后再动手
- **每次设计决策变更后，无需等用户提醒，主动同步更新以下三处**：
  1. `analysis/requirements.md`
  2. `docs/菜单助手-产品需求文档.md`
  3. `copilot-instructions.md` 的「业务设计决策」节

## 重要规则

- 优先遵循instructions文档描述的规范

## 项目运行约定

- **Git**：不要执行任何 git 命令，由用户手动提交
- **数据库迁移**：后端启动时自动调用 `MigrateAsync()`，无需手动执行 `dotnet ef database update`；生成迁移文件后告知用户重启后端即可
- **后端端口**：`http://localhost:5511`（见 launchSettings.json）
- **CORS**：从 `appsettings.Development.json` 的 `Cors:AllowedOrigins` 读取，已配置 `localhost:5173`、`localhost:8080`
- **前端框架**：Wot UI（npm 包名 `wot-design-uni`），easycom 已在 `pages.json` 配置
- **前端 BASE_URL**：`http://localhost:5511`（见 `src/frontend/utils/api.js`）

## 最佳实践

- 优先使用主构造函数
- 使用各类组件时，优先使用await关键字，使用Async方法

## 根据需要按照下列顺序完成工作

- 定义聚合、实体
- 定义领域事件
- 创建仓储接口与仓储实现
- 配置实体映射
- 定义命令与命令处理器
- 定义查询与查询处理器
- 定义Endpoints
- 定义领域事件处理器
- 定义集成事件
- 定义集成事件转换器
- 定义集成事件处理器

## 项目结构

```
MenuHelper.sln
├── src/
│   ├── MenuHelper.Domain/         # 领域层 - 聚合根、实体、领域事件
│   ├── MenuHelper.Infrastructure/ # 基础设施层 - EF配置、仓储接口、仓储实现
│   └── MenuHelper.Web/           # 表现层 - API、应用服务
└── test/                            # 测试项目
    ├── MenuHelper.Domain.UnitTests/         # 领域层测试项目
    ├── MenuHelper.Infrastructure.UnitTests/ # 基础设施层测试项目
    └── MenuHelper.Web.UnitTests/           # 表现层测试项目
```

**分层依赖关系：** Web → Infrastructure → Domain (严格单向依赖)



## 对于具体的开发工作，请参考以下详细指令文件：

### 聚合与领域层
- **聚合根开发**: 参考 `.github/instructions/aggregate.instructions.md`
- **领域事件定义**: 参考 `.github/instructions/domain-event.instructions.md`

### 数据访问层
- **仓储实现**: 参考 `.github/instructions/repository.instructions.md`
- **实体配置**: 参考 `.github/instructions/entity-configuration.instructions.md`
- **数据库上下文**: 参考 `.github/instructions/dbcontext.instructions.md`

### 应用服务层
- **命令处理**: 参考 `.github/instructions/command.instructions.md`
- **查询处理**: 参考 `.github/instructions/query.instructions.md`
- **领域事件处理**: 参考 `.github/instructions/domain-event-handler.instructions.md`

### API表现层
- **API端点**: 参考 `.github/instructions/endpoint.instructions.md`

### 集成事件处理
- **集成事件**: 参考 `.github/instructions/integration-event.instructions.md`
- **集成事件转换器**: 参考 `.github/instructions/integration-event-converter.instructions.md`
- **集成事件处理器**: 参考 `.github/instructions/integration-event-handler.instructions.md`

### 测试
- **单元测试**: 参考 `.github/instructions/unit-testing.instructions.md`

### 最佳实践
（遵循各模块对应的 *.instructions.md 文档；本节不再另行维护“通用最佳实践”文件以避免重复和漂移。）

## 业务设计决策（已确认）

### 每日库存盘点（REQ-010）
- **适用食材**：面粉、鸡蛋、白饼、腊汁肉等**可以称重/数数**的食材
- **不适用**：黄瓜、小葱等蔬菜（店主目测判断够不够，不称重，不强制精确录入数量）
- ⚠️ **设计约定**：涉及库存盘点的新功能，必须区分这两类食材，**不要为"不适用"的食材设计数量追踪字段**（教训：`StockedOnDate` 字段最终被删除，原因是蔬菜类食材根本不追踪批次）

### 新鲜度预警（REQ-012）
- **实现方式**：基于账单记录中该食材**最近一次进货日期**计算存放天数，不依赖库存盘点数量推算
- **优先级**：P2，依赖账单模块（Issue #23）完成后实现
- **`InventoryCheckItem` 不含 `StockedOnDate` 字段**，新鲜度计算不在盘点聚合内进行

---



### 文件组织
- **聚合根** → `src/MenuHelper.Domain/AggregatesModel/{AggregateFolder}/`
- **领域事件** → `src/MenuHelper.Domain/DomainEvents/`
- **仓储** → `src/MenuHelper.Infrastructure/Repositories/`
- **实体配置** → `src/MenuHelper.Infrastructure/EntityConfigurations/`
- **命令与命令处理器** → `src/MenuHelper.Web/Application/Commands/`
- **查询与查询处理器** → `src/MenuHelper.Web/Application/Queries/`
- **API端点** → `src/MenuHelper.Web/Endpoints/`
- **领域事件处理器** → `src/MenuHelper.Web/Application/DomainEventHandlers/`
- **集成事件** → `src/MenuHelper.Web/Application/IntegrationEvents/`
- **集成事件转换器** → `src/MenuHelper.Web/Application/IntegrationEventConverters/`
- **集成事件处理器** → `src/MenuHelper.Web/Application/IntegrationEventHandlers/`

### 强制性要求
- ✅ 所有聚合根使用强类型ID，且**不手动赋值ID**（依赖EF值生成器）
- ✅ 所有命令都要有对应的验证器
- ✅ 领域事件在聚合发生改变时发布,仅聚合和实体可以发出领域事件
- ✅ 遵循分层架构依赖关系 (Web → Infrastructure → Domain)
- ✅ 使用KnownException处理已知业务异常
- ✅ 命令处理器**不调用SaveChanges**（框架自动处理）
- ✅ 仓储必须使用**异步方法**（GetAsync、AddAsync等）

### 关键技术要求
- **验证器**: 必须继承 `AbstractValidator<T>` 而不是 `Validator<T>`
- **领域事件处理器**: 实现 `Handle()` 方法而不是 `HandleAsync()`
- **FastEndpoints**: 使用构造函数注入 `IMediator`，使用 `Send.OkAsync()` 和 `.AsResponseData()`；端点采用属性特性配置（如 `[HttpPost]`、`[AllowAnonymous]`、`[Tags]`），不使用 `Configure()`
- **强类型ID**: 直接使用类型，避免 `.Value` 属性，依赖隐式转换
- **仓储**: 通过构造函数参数访问 `ApplicationDbContext`，所有操作必须异步
- **ID生成**: 使用EF Core值生成器，聚合根构造函数不设置ID

## 异常处理原则

### KnownException使用规范
在需要抛出业务异常的地方，必须使用 `KnownException` 而不是普通的 `Exception`：

**正确示例：**
```csharp
// 在聚合根中
public void OrderPaid()
{
    if (Paid)
    {
        throw new KnownException("Order has been paid");
    }
    // 业务逻辑...
}

// 在命令处理器中
public async Task<OrderId> Handle(OrderPaidCommand request, CancellationToken cancellationToken)
{
    var order = await orderRepository.GetAsync(request.OrderId, cancellationToken) ??
                throw new KnownException($"未找到订单，OrderId = {request.OrderId}");
    order.OrderPaid();
    return order.Id;
}
```

**框架集成：**
- `KnownException` 会被框架自动转换为合适的HTTP状态码
- 异常消息会直接返回给客户端
- 支持本地化和错误码定制

## 常见using引用指南

### GlobalUsings.cs配置
各层的常用引用已在GlobalUsings.cs中全局定义：

**Web层** (`src/MenuHelper.Web/GlobalUsings.cs`):
- `global using FluentValidation;` - 验证器
- `global using MediatR;` - 命令处理器  
- `global using NetCorePal.Extensions.Primitives;` - KnownException等
- `global using FastEndpoints;` - API端点
- `global using NetCorePal.Extensions.Dto;` - ResponseData
- `global using NetCorePal.Extensions.Domain;` - 领域事件处理器

**Infrastructure层** (`src/MenuHelper.Infrastructure/GlobalUsings.cs`):
- `global using Microsoft.EntityFrameworkCore;` - EF Core
- `global using Microsoft.EntityFrameworkCore.Metadata.Builders;` - 实体配置
- `global using NetCorePal.Extensions.Primitives;` - 基础类型

**Domain层** (`src/MenuHelper.Domain/GlobalUsings.cs`):
- `global using NetCorePal.Extensions.Domain;` - 领域基础类型
- `global using NetCorePal.Extensions.Primitives;` - 强类型ID等

**Tests层** (`test/*/GlobalUsings.cs`):
- `global using Xunit;` - 测试框架
- `global using NetCorePal.Extensions.Primitives;` - 测试中的异常处理

### 常见手动using引用
当GlobalUsings未覆盖时，需要手动添加：

**查询处理器**:
```csharp
using MenuHelper.Domain.AggregatesModel.{AggregateFolder};
using MenuHelper.Infrastructure;
```

**实体配置**:
```csharp
using MenuHelper.Domain.AggregatesModel.{AggregateFolder};
```

**端点**:
```csharp
using MenuHelper.Domain.AggregatesModel.{AggregateFolder};
using MenuHelper.Web.Application.Commands.{FeatureFolder};
using MenuHelper.Web.Application.Queries.{FeatureFolder};
```

---

## 产品需求文档

# 🥘 菜单助手 - 产品需求文档

> 一款帮助餐饮店主每日开菜单、管理库存、对接供应商的微信小程序

---

## 一、背景与目标

店主经营一家主营凉皮、肉夹馍、石锅饭的小店，每天需要手工开菜单备货（约20-30种原材料），目前纸质记录，容易漏开导致第二天缺货。

**目标：** 用微信小程序替代纸质菜单，实现智能备货建议、自动生成分类采购单、一键发给供应商。

---

## 二、用户

- 单人使用（店主本人）

---

## 三、功能模块

### 3.1 菜品与原材料管理

- 配置菜品（凉皮 / 肉夹馍 / 石锅饭）
- 每个菜品关联所需原材料及用量
  - 例：肉夹馍 = 馍（固定1个）+ 腊汁肉（非固定）+ 辣椒
- 原材料支持标记所属**菜品分类**：
  - 【凉皮类】【肉夹馍类】【石锅饭类】【通用食材】
- 原材料标记**消耗方式**（独立于菜品分类，两套属性并存）：
  - 按库存盘点：白饼、鸡蛋、面粉、肉等可盘点食材，(期初+进货-期末)×单价计算成本
  - 按周期摊销：盐、香料、料子等难以精确盘点的调料，总价÷周期天数每日均摊
  - 按销量推算：固定用量食材，接入收钱吧后启用
- 每种原材料可绑定供应商（或标记为"自购"）
- 设置原材料的：
  - 安全库存线（低于此值提醒）
  - 默认备货周期（如：凉皮面粉默认每2天进一次）
  - 最长存放天数（超时提醒新鲜度）
- **价格快照机制**：
  - 每种原材料预设默认单价，仅作为采购时的默认填入值
  - 修改预设单价只影响未来采购，历史账单价格永久锁定不变

---

### 3.2 原材料三种管理模式

根据食材特性，分为三种补货推算模式：

#### 模式一：固定消耗（如白饼、鸡蛋）
- 消耗关系精确：1份肉夹馍 = 1个白饼
- **前期**（无订单数据）：人工盘点库存
- **后期**（接入收钱吧）：销量 × 固定用量，精确推算库存消耗

#### 模式二：非固定消耗（如腊汁肉、凉皮面粉）
- 每份用量不固定（手工分量），无法精确计算
- **前期**：人工盘点 + 按进货时间间隔学习
- **后期**（接入收钱吧）：自动切换为销量推算
- 切换后保留时间间隔历史数据作辅助参考

#### 模式三：调料类（如盐、油、料子）
- 用量难以精确追踪，按时间周期提醒
- **自学习提醒周期**：
  - 冷启动：默认5天提醒一次
  - 逐步学习：根据历史实际进货间隔，用加权移动平均修正周期（新周期 = 最近一次×50% + 上上次×30% + 再上次×20%）

---

### 3.3 每日库存盘点

- 每天营业结束后快速录入剩余库存数量
- 支持快速数字键盘输入
- **定时提醒**：每天固定时间推送提醒"该盘点库存了"
- **新鲜度预警**：若某原材料已存放超过设定天数，发出提示

---

### 3.4 智能开菜单（核心功能）

#### 天气获取
- 自动定位获取当前位置
- 拉取未来3天天气预报
- 权重：明天（主要）> 后天（次要）> 第三天（参考）

#### AI智能推荐面板
综合以下因素给出补货建议：库存剩余、备货周期、天气、星期/节假日、季节、销量进度（接入收钱吧后）

- 每条推荐显示理由
- 店主可勾选/取消/修改数量
- AI根据店主历史选择习惯，逐渐修正推荐准确性

---

### 3.5 生成分类采购菜单

确认推荐列表后，自动生成按菜品分类整理的采购清单（凉皮类/肉夹馍类/石锅饭类/通用食材），多菜品共用原材料合并到通用食材。

---

### 3.6 供应商管理与发单

#### 供应商配置
- 管理供应商列表（姓名 / 联系方式 / 负责品类）
- 每种原材料绑定供应商（或标记为"自购"）

#### 发单功能
- 采购菜单按供应商自动拆分
- **一键复制**对应供应商的订单文本，手动粘贴发微信

#### 供应商对账本
- 每种原材料预设单价，采购时自动填入（可临时修改当次价格）
- 每次进货自动生成账单记录（价格快照，历史永久不变）
- 供应商欠款总览，支持一键结清

---

### 3.7 财务统计

#### 成本计入原则
- **权责发生制**：进货时计入成本，与结账时间无关
- **按库存盘点**食材：今日消耗=(期初+进货-期末)×单价
- **按周期摊销**食材：按历史进货周期每日均摊

#### 采购支出统计（三个维度）
- 按菜品分类（凉皮类/肉夹馍类/石锅饭类/通用食材）
- 按供应商
- 按单个原材料（含历史进货价格走势）
- 时间维度：今日 / 本周 / 本月

#### 营业收入
- 每日手动录入当天营业额
- 统计维度：今日 / 本周 / 本月

#### 利润分析
- 今日毛利润 = 今日营业收入 - 今日按库存盘点成本 - 今日应摊销成本
- 日 / 周 / 月 维度展示

---

### 3.8 提醒系统

| 提醒类型 | 触发条件 | 时机 |
|---------|---------|------|
| 库存盘点提醒 | 每天固定时间 | Push通知 |
| 新鲜度预警 | 存放超过设定天数 | 开菜单时推荐面板 |
| 补货周期提醒 | 超过历史平均周期未进货 | 开菜单时推荐面板 |
| 调料周期提醒 | 距上次进货超过学习周期 | 开菜单时推荐面板 |
| 节假日/周末提醒 | 明日为节假日/周末 | 开菜单时推荐面板 |

---

### 3.9 第三方系统对接（后期）

- 对接收钱吧扫码点餐系统，自动获取各菜品实时销量
- 固定消耗食材自动推算库存消耗
- 非固定消耗食材切换为销量学习模式

---

## 四、技术栈

| 层次 | 技术选型 |
|------|---------|
| **前端** | UniApp（发布微信小程序，可扩展至支付宝/H5） |
| **后端** | ASP.NET Core Web API |
| **ORM** | EF Core |
| **数据库** | MySQL |
| **天气数据** | 第三方天气API（如和风天气，自动定位） |

---

## 五、MVP 优先级

| 优先级 | 功能 |
|--------|------|
| 🔴 P0 必做 | 原材料管理、每日人工盘点、生成分类菜单、供应商一键复制 |
| 🟡 P1 重要 | AI推荐面板、天气备货建议、定时提醒、供应商对账本 |
| 🟢 P2 增强 | 财务统计（含摊销）、新鲜度预警、自学习周期修正 |
| 🔵 P3 远期 | 对接收钱吧、自动库存推算、销量驱动补货 |

---

## 六、已解决的逻辑冲突

| 冲突 | 解决方案 |
|------|---------|
| 成本计入时机 | 进货时计入（权责发生制），与结账时间无关 |
| 两套分类体系并存 | 菜品分类（统计用）与消耗方式（利润用）独立存在，互不影响 |
| 摊销超期处理 | 进货当天回算实际天数，修正之前的摊销数据 |
| 前期固定/非固定食材区分 | 前期统一人工盘点，接入收钱吧后自动区分 |
| 两阶段学习切换 | 接入收钱吧后自动切换，保留时间数据作辅助参考 |
