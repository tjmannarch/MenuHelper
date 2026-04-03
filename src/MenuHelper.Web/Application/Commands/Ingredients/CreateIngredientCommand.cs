using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Ingredients;

public record CreateIngredientCommand(
    string Name,
    string Unit,
    IngredientCategory Category,
    ConsumptionType ConsumptionType,
    decimal? DefaultUnitPrice = null,
    SupplierId? SupplierId = null,
    decimal? SafetyStockLevel = null,
    int? RestockCycleDays = null,
    int? MaxShelfDays = null) : ICommand<IngredientId>;

public class CreateIngredientCommandValidator : AbstractValidator<CreateIngredientCommand>
{
    public CreateIngredientCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("原材料名称不能为空").MaximumLength(100);
        RuleFor(x => x.Unit).NotEmpty().WithMessage("计量单位不能为空").MaximumLength(20);
        RuleFor(x => x.DefaultUnitPrice).GreaterThanOrEqualTo(0).When(x => x.DefaultUnitPrice.HasValue).WithMessage("默认单价不能小于0");
        RuleFor(x => x.SafetyStockLevel).GreaterThanOrEqualTo(0).When(x => x.SafetyStockLevel.HasValue).WithMessage("安全库存线不能小于0");
        RuleFor(x => x.RestockCycleDays).GreaterThanOrEqualTo(1).When(x => x.RestockCycleDays.HasValue).WithMessage("备货周期不能小于1天");
        RuleFor(x => x.MaxShelfDays).GreaterThanOrEqualTo(1).When(x => x.MaxShelfDays.HasValue).WithMessage("最长存放天数不能小于1天");
    }
}

public class CreateIngredientCommandHandler(IIngredientRepository ingredientRepository)
    : ICommandHandler<CreateIngredientCommand, IngredientId>
{
    public async Task<IngredientId> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = new Ingredient(
            request.Name, request.Unit, request.Category, request.ConsumptionType,
            request.DefaultUnitPrice, request.SafetyStockLevel, request.RestockCycleDays, request.MaxShelfDays);

        if (request.SupplierId != null)
            ingredient.BindSupplier(request.SupplierId);

        await ingredientRepository.AddAsync(ingredient, cancellationToken);
        return ingredient.Id;
    }
}
