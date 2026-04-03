using MenuHelper.Domain.AggregatesModel.InventoryCheckAggregate;

namespace MenuHelper.Domain.DomainEvents;

public record InventoryCheckCreatedDomainEvent(InventoryCheck InventoryCheck) : IDomainEvent;
public record InventoryCheckItemsUpdatedDomainEvent(InventoryCheck InventoryCheck) : IDomainEvent;
