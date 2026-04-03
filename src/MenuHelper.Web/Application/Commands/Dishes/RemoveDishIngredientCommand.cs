using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Dishes;

public record RemoveDishIngredientCommand(DishId DishId, DishIngredientId DishIngredientId) : ICommand;

public class RemoveDishIngredientCommandValidator : AbstractValidator<RemoveDishIngredientCommand>
{
    public RemoveDishIngredientCommandValidator()
    {
        RuleFor(x => x.DishId).NotEmpty();
        RuleFor(x => x.DishIngredientId).NotEmpty();
    }
}

public class RemoveDishIngredientCommandHandler(IDishRepository dishRepository)
    : ICommandHandler<RemoveDishIngredientCommand>
{
    public async Task Handle(RemoveDishIngredientCommand request, CancellationToken cancellationToken)
    {
        var dish = await dishRepository.GetWithIngredientsAsync(request.DishId, cancellationToken)
            ?? throw new KnownException($"未找到菜品，DishId = {request.DishId}");
        dish.RemoveIngredient(request.DishIngredientId);
    }
}
