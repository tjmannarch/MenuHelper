using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Dishes;

public record CreateDishCommand(string Name) : ICommand<DishId>;

public class CreateDishCommandValidator : AbstractValidator<CreateDishCommand>
{
    public CreateDishCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("菜品名称不能为空").MaximumLength(100);
    }
}

public class CreateDishCommandHandler(IDishRepository dishRepository)
    : ICommandHandler<CreateDishCommand, DishId>
{
    public async Task<DishId> Handle(CreateDishCommand request, CancellationToken cancellationToken)
    {
        var dish = new Dish(request.Name);
        await dishRepository.AddAsync(dish, cancellationToken);
        return dish.Id;
    }
}
