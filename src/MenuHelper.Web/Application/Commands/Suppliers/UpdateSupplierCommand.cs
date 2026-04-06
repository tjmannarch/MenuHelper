using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Suppliers;

public record UpdateSupplierCommand(SupplierId SupplierId, string Name, string? Phone, string? Remark) : ICommand;

public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(x => x.SupplierId).NotEmpty().WithMessage("供应商ID不能为空");
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

public class UpdateSupplierCommandHandler(ISupplierRepository supplierRepository)
    : ICommandHandler<UpdateSupplierCommand>
{
    public async Task Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetAsync(request.SupplierId, cancellationToken)
            ?? throw new KnownException($"未找到供应商，Id = {request.SupplierId}");
        if (supplier.Deleted)
            throw new KnownException("供应商已删除，无法更新");
        supplier.Update(request.Name, request.Phone, request.Remark);
    }
}
