using MenuHelper.Domain.AggregatesModel.SupplierAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Purchases;

public record SettleSupplierCommand(SupplierId SupplierId) : ICommand;

public class SettleSupplierCommandValidator : AbstractValidator<SettleSupplierCommand>
{
    public SettleSupplierCommandValidator()
    {
        RuleFor(x => x.SupplierId).NotEmpty().WithMessage("供应商ID不能为空");
    }
}

public class SettleSupplierCommandHandler(IPurchaseRepository purchaseRepository)
    : ICommandHandler<SettleSupplierCommand>
{
    public async Task Handle(SettleSupplierCommand request, CancellationToken cancellationToken)
    {
        var unpaidPurchases = await purchaseRepository.GetUnpaidBySupplierAsync(request.SupplierId, cancellationToken);
        if (unpaidPurchases.Count == 0)
            throw new KnownException("该供应商没有待结算的进货记录");
        foreach (var purchase in unpaidPurchases)
        {
            purchase.Pay();
        }
    }
}
