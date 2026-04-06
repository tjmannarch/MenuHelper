using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.InventoryCheckAggregate;
using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MenuHelper.Web.Application.Queries.InventoryChecks;

public record GetInventoryCheckQuery(DateOnly CheckDate) : IQuery<InventoryCheckDto>;

public class GetInventoryCheckQueryValidator : AbstractValidator<GetInventoryCheckQuery>
{
    public GetInventoryCheckQueryValidator()
    {
        RuleFor(x => x.CheckDate).NotEmpty().WithMessage("盘点日期不能为空");
    }
}

public class GetInventoryCheckQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetInventoryCheckQuery, InventoryCheckDto>
{
    public async Task<InventoryCheckDto> Handle(GetInventoryCheckQuery request, CancellationToken cancellationToken)
    {
        var dto = await context.InventoryChecks
            .Where(x => x.CheckDate == request.CheckDate && !x.Deleted)
            .Select(x => new InventoryCheckDto(
                x.Id,
                x.CheckDate,
                x.Items.Select(i => new InventoryCheckItemDto(i.Id, i.IngredientId, i.Quantity)).ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        // 当天无记录时返回空明细，前端展示空表单
        return dto ?? new InventoryCheckDto(null, request.CheckDate, []);
    }
}

public record InventoryCheckDto(InventoryCheckId? Id, DateOnly CheckDate, List<InventoryCheckItemDto> Items);

public record InventoryCheckItemDto(InventoryCheckItemId Id, IngredientId IngredientId, decimal Quantity);
