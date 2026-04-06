using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Suppliers;

public record CreateSupplierCommand(string Name, string? Phone, string? Remark) : ICommand<SupplierId>;

public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("供应商名称不能为空")
            .MaximumLength(100).WithMessage("供应商名称不能超过100个字符");
        RuleFor(x => x.Phone)
            .MaximumLength(50).WithMessage("联系电话不能超过50个字符")
            .When(x => x.Phone != null);
        RuleFor(x => x.Remark)
            .MaximumLength(500).WithMessage("备注不能超过500个字符")
            .When(x => x.Remark != null);
    }
}

public class CreateSupplierCommandHandler(ISupplierRepository supplierRepository)
    : ICommandHandler<CreateSupplierCommand, SupplierId>
{
    public async Task<SupplierId> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = new Supplier(request.Name, request.Phone, request.Remark);
        await supplierRepository.AddAsync(supplier, cancellationToken);
        return supplier.Id;
    }
}
