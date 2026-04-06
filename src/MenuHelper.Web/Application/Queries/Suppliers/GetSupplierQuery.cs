using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MenuHelper.Web.Application.Queries.Suppliers;

public record GetSupplierQuery(SupplierId SupplierId) : IQuery<SupplierDto>;

public class GetSupplierQueryValidator : AbstractValidator<GetSupplierQuery>
{
    public GetSupplierQueryValidator()
    {
        RuleFor(x => x.SupplierId).NotEmpty().WithMessage("供应商ID不能为空");
    }
}

public class GetSupplierQueryHandler(ApplicationDbContext context) : IQueryHandler<GetSupplierQuery, SupplierDto>
{
    public async Task<SupplierDto> Handle(GetSupplierQuery request, CancellationToken cancellationToken)
    {
        return await context.Set<Supplier>()
            .Where(x => x.Id == request.SupplierId && !x.Deleted)
            .Select(x => new SupplierDto(x.Id, x.Name, x.Phone, x.Remark))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new KnownException($"未找到供应商，Id = {request.SupplierId}");
    }
}

public record SupplierDto(SupplierId Id, string Name, string? Phone, string? Remark);
