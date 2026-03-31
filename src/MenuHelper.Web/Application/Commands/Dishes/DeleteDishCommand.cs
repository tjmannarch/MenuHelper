using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Dishes;

public record DeleteDishCommand(DishId DishId) : ICommand;

public class DeleteDishCommandValidator : AbstractValidator<DeleteDishCommand>
{
    public DeleteDishCommandValidator()
    {
        RuleFor(x => x.DishId).NotEmpty();
    }
}

public class DeleteDishCommandHandler(IDishRepository dishRepository)
    : ICommandHandler<DeleteDishCommand>
{
    public async Task Handle(DeleteDishCommand request, CancellationToken cancellationToken)
    {
        var dish = await dishRepository.GetAsync(request.DishId, cancellationToken)
            ?? throw new KnownException($"未找到菜品，DishId = {request.DishId}");
        dish.Delete();
    }
}
