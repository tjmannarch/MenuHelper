using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Ingredients;

public record UpdateIngredientCommand(
    IngredientId IngredientId,
    string Name,
    string Unit,
    IngredientCategory Category,
    ConsumptionType ConsumptionType,
    decimal DefaultUnitPrice,
    decimal? SafetyStockLevel = null,
    int? RestockCycleDays = null,
    int? MaxShelfDays = null) : ICommand;

public class UpdateIngredientCommandValidator : AbstractValidator<UpdateIngredientCommand>
{
    public UpdateIngredientCommandValidator()
    {
        RuleFor(x => x.IngredientId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("原材料名称不能为空").MaximumLength(100);
        RuleFor(x => x.Unit).NotEmpty().WithMessage("计量单位不能为空").MaximumLength(20);
        RuleFor(x => x.DefaultUnitPrice).GreaterThanOrEqualTo(0).WithMessage("默认单价不能小于0");
        RuleFor(x => x.SafetyStockLevel).GreaterThanOrEqualTo(0).When(x => x.SafetyStockLevel.HasValue).WithMessage("安全库存线不能小于0");
        RuleFor(x => x.RestockCycleDays).GreaterThanOrEqualTo(1).When(x => x.RestockCycleDays.HasValue).WithMessage("备货周期不能小于1天");
        RuleFor(x => x.MaxShelfDays).GreaterThanOrEqualTo(1).When(x => x.MaxShelfDays.HasValue).WithMessage("最长存放天数不能小于1天");
    }
}

public class UpdateIngredientCommandHandler(IIngredientRepository ingredientRepository)
    : ICommandHandler<UpdateIngredientCommand>
{
    public async Task Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await ingredientRepository.GetAsync(request.IngredientId, cancellationToken)
            ?? throw new KnownException($"未找到原材料，IngredientId = {request.IngredientId}");

        ingredient.UpdateBasicInfo(request.Name, request.Unit, request.Category, request.ConsumptionType,
            request.DefaultUnitPrice, request.SafetyStockLevel, request.RestockCycleDays, request.MaxShelfDays);
    }
}
