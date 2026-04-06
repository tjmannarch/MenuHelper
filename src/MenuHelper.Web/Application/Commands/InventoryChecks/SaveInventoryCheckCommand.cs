using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.InventoryCheckAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.InventoryChecks;

public record InventoryCheckItemInput(IngredientId IngredientId, decimal Quantity);

public record SaveInventoryCheckCommand(DateOnly CheckDate, List<InventoryCheckItemInput> Items)
    : ICommand<InventoryCheckId>;

public class SaveInventoryCheckCommandValidator : AbstractValidator<SaveInventoryCheckCommand>
{
    public SaveInventoryCheckCommandValidator()
    {
        RuleFor(x => x.CheckDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("盘点日期不能是未来日期");

        RuleFor(x => x.Items)
            .NotNull()
            .WithMessage("盘点明细不能为 null");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.IngredientId).NotEmpty().WithMessage("原材料ID不能为空");
                item.RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).WithMessage("库存数量不能小于0");
            });

        RuleFor(x => x.Items)
            .Must(items => items == null || items.Select(i => i.IngredientId).Distinct().Count() == items.Count)
            .WithMessage("同一盘点中不允许重复录入同一原材料");
    }
}

public class SaveInventoryCheckCommandHandler(IInventoryCheckRepository inventoryCheckRepository)
    : ICommandHandler<SaveInventoryCheckCommand, InventoryCheckId>
{
    public async Task<InventoryCheckId> Handle(SaveInventoryCheckCommand request, CancellationToken cancellationToken)
    {
        var tuples = request.Items.Select(x => (x.IngredientId, x.Quantity));

        var existing = await inventoryCheckRepository.GetByDateAsync(request.CheckDate, cancellationToken);
        if (existing != null)
        {
            existing.SetItems(tuples);
            return existing.Id;
        }

        var check = new InventoryCheck(request.CheckDate, tuples);
        await inventoryCheckRepository.AddAsync(check, cancellationToken);
        return check.Id;
    }
}
