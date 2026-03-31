using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Dishes;

public record UpdateDishIngredientCommand(
    DishId DishId,
    IngredientId IngredientId,
    QuantityType QuantityType,
    decimal? FixedQuantity = null) : ICommand;

public class UpdateDishIngredientCommandValidator : AbstractValidator<UpdateDishIngredientCommand>
{
    public UpdateDishIngredientCommandValidator()
    {
        RuleFor(x => x.DishId).NotEmpty();
        RuleFor(x => x.IngredientId).NotEmpty();
        RuleFor(x => x.FixedQuantity)
            .GreaterThan(0).When(x => x.QuantityType == QuantityType.Fixed)
            .WithMessage("固定用量必须大于0");
    }
}

public class UpdateDishIngredientCommandHandler(IDishRepository dishRepository)
    : ICommandHandler<UpdateDishIngredientCommand>
{
    public async Task Handle(UpdateDishIngredientCommand request, CancellationToken cancellationToken)
    {
        var dish = await dishRepository.GetAsync(request.DishId, cancellationToken)
            ?? throw new KnownException($"未找到菜品，DishId = {request.DishId}");
        dish.UpdateIngredient(request.IngredientId, request.QuantityType, request.FixedQuantity);
    }
}
