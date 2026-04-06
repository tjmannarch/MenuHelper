using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Suppliers;

public record DeleteSupplierCommand(SupplierId SupplierId) : ICommand;

public class DeleteSupplierCommandValidator : AbstractValidator<DeleteSupplierCommand>
{
    public DeleteSupplierCommandValidator()
    {
        RuleFor(x => x.SupplierId).NotEmpty().WithMessage("供应商ID不能为空");
    }
}

public class DeleteSupplierCommandHandler(ISupplierRepository supplierRepository)
    : ICommandHandler<DeleteSupplierCommand>
{
    public async Task Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = await supplierRepository.GetAsync(request.SupplierId, cancellationToken)
            ?? throw new KnownException($"未找到供应商，Id = {request.SupplierId}");
        supplier.Delete();
    }
}
