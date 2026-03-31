using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MenuHelper.Web.Application.Queries.Ingredients;

public record GetIngredientQuery(IngredientId IngredientId) : IQuery<IngredientDetailDto>;

public class GetIngredientQueryValidator : AbstractValidator<GetIngredientQuery>
{
    public GetIngredientQueryValidator()
    {
        RuleFor(x => x.IngredientId).NotEmpty();
    }
}

public class GetIngredientQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetIngredientQuery, IngredientDetailDto>
{
    public async Task<IngredientDetailDto> Handle(GetIngredientQuery request, CancellationToken cancellationToken)
    {
        return await context.Ingredients
            .Where(x => x.Id == request.IngredientId && !x.Deleted)
            .Select(x => new IngredientDetailDto(
                x.Id, x.Name, x.Unit, x.Category, x.ConsumptionType,
                x.SupplierId, x.SafetyStockLevel, x.RestockCycleDays,
                x.MaxShelfDays, x.DefaultUnitPrice))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KnownException($"未找到原材料，IngredientId = {request.IngredientId}");
    }
}

public record IngredientDetailDto(
    IngredientId Id,
    string Name,
    string Unit,
    IngredientCategory Category,
    ConsumptionType ConsumptionType,
    SupplierId? SupplierId,
    decimal? SafetyStockLevel,
    int? RestockCycleDays,
    int? MaxShelfDays,
    decimal DefaultUnitPrice);
