using MenuHelper.Domain.AggregatesModel.PurchaseAggregate;

namespace MenuHelper.Domain.DomainEvents;

public record PurchaseCreatedDomainEvent(Purchase Purchase) : IDomainEvent;
public record PurchasePaidDomainEvent(Purchase Purchase) : IDomainEvent;
