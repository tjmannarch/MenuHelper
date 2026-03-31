using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Ingredients;

public record BindIngredientSupplierCommand(IngredientId IngredientId, SupplierId? SupplierId) : ICommand;

public class BindIngredientSupplierCommandValidator : AbstractValidator<BindIngredientSupplierCommand>
{
    public BindIngredientSupplierCommandValidator()
    {
        RuleFor(x => x.IngredientId).NotEmpty();
    }
}

public class BindIngredientSupplierCommandHandler(IIngredientRepository ingredientRepository)
    : ICommandHandler<BindIngredientSupplierCommand>
{
    public async Task Handle(BindIngredientSupplierCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await ingredientRepository.GetAsync(request.IngredientId, cancellationToken)
            ?? throw new KnownException($"未找到原材料，IngredientId = {request.IngredientId}");

        ingredient.BindSupplier(request.SupplierId);
    }
}
