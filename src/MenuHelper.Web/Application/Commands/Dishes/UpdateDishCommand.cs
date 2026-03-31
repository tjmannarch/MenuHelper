using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Dishes;

public record UpdateDishCommand(DishId DishId, string Name) : ICommand;

public class UpdateDishCommandValidator : AbstractValidator<UpdateDishCommand>
{
    public UpdateDishCommandValidator()
    {
        RuleFor(x => x.DishId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("菜品名称不能为空").MaximumLength(100);
    }
}

public class UpdateDishCommandHandler(IDishRepository dishRepository)
    : ICommandHandler<UpdateDishCommand>
{
    public async Task Handle(UpdateDishCommand request, CancellationToken cancellationToken)
    {
        var dish = await dishRepository.GetAsync(request.DishId, cancellationToken)
            ?? throw new KnownException($"未找到菜品，DishId = {request.DishId}");
        dish.UpdateName(request.Name);
    }
}
