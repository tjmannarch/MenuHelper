using MenuHelper.Domain.AggregatesModel.SupplierAggregate;

namespace MenuHelper.Domain.DomainEvents;

public record SupplierCreatedDomainEvent(Supplier Supplier) : IDomainEvent;
public record SupplierUpdatedDomainEvent(Supplier Supplier) : IDomainEvent;
public record SupplierDeletedDomainEvent(Supplier Supplier) : IDomainEvent;
