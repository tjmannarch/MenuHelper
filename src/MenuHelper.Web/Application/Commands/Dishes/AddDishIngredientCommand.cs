using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Dishes;

public record AddDishIngredientCommand(
    DishId DishId,
    IngredientId IngredientId,
    QuantityType QuantityType,
    decimal? FixedQuantity = null) : ICommand;

public class AddDishIngredientCommandValidator : AbstractValidator<AddDishIngredientCommand>
{
    public AddDishIngredientCommandValidator()
    {
        RuleFor(x => x.DishId).NotEmpty();
        RuleFor(x => x.IngredientId).NotEmpty();
        RuleFor(x => x.FixedQuantity)
            .GreaterThan(0).When(x => x.QuantityType == QuantityType.Fixed)
            .WithMessage("固定用量必须大于0");
    }
}

public class AddDishIngredientCommandHandler(IDishRepository dishRepository, IIngredientRepository ingredientRepository)
    : ICommandHandler<AddDishIngredientCommand>
{
    public async Task Handle(AddDishIngredientCommand request, CancellationToken cancellationToken)
    {
        var dish = await dishRepository.GetAsync(request.DishId, cancellationToken)
            ?? throw new KnownException($"未找到菜品，DishId = {request.DishId}");

        _ = await ingredientRepository.GetAsync(request.IngredientId, cancellationToken)
            ?? throw new KnownException($"未找到原材料，IngredientId = {request.IngredientId}");

        dish.AddIngredient(request.IngredientId, request.QuantityType, request.FixedQuantity);
    }
}
