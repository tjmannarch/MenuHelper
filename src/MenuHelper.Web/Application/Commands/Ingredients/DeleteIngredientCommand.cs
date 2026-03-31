using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Ingredients;

public record DeleteIngredientCommand(IngredientId IngredientId) : ICommand;

public class DeleteIngredientCommandValidator : AbstractValidator<DeleteIngredientCommand>
{
    public DeleteIngredientCommandValidator()
    {
        RuleFor(x => x.IngredientId).NotEmpty();
    }
}

public class DeleteIngredientCommandHandler(
    IIngredientRepository ingredientRepository)
    : ICommandHandler<DeleteIngredientCommand>
{
    public async Task Handle(DeleteIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await ingredientRepository.GetAsync(request.IngredientId, cancellationToken)
            ?? throw new KnownException($"未找到原材料，IngredientId = {request.IngredientId}");

        var inUse = await ingredientRepository.IsIngredientInUseAsync(request.IngredientId, cancellationToken);
        if (inUse) throw new KnownException("该原材料已被菜品关联，请先移除相关菜品关联后再删除");

        ingredient.Delete();
    }
}
