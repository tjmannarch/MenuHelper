using MenuHelper.Domain.AggregatesModel.PurchaseAggregate;
using MenuHelper.Infrastructure.Repositories;

namespace MenuHelper.Web.Application.Commands.Purchases;

public record PayPurchaseCommand(PurchaseId PurchaseId) : ICommand;

public class PayPurchaseCommandValidator : AbstractValidator<PayPurchaseCommand>
{
    public PayPurchaseCommandValidator()
    {
        RuleFor(x => x.PurchaseId).NotEmpty().WithMessage("进货记录ID不能为空");
    }
}

public class PayPurchaseCommandHandler(IPurchaseRepository purchaseRepository)
    : ICommandHandler<PayPurchaseCommand>
{
    public async Task Handle(PayPurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = await purchaseRepository.GetAsync(request.PurchaseId, cancellationToken)
            ?? throw new KnownException($"未找到进货记录，Id = {request.PurchaseId}");
        purchase.Pay();
    }
}
