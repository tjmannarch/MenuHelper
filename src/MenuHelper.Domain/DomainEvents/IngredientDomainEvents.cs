using MenuHelper.Domain.AggregatesModel.IngredientAggregate;

namespace MenuHelper.Domain.DomainEvents;

public record IngredientCreatedDomainEvent(Ingredient Ingredient) : IDomainEvent;
public record IngredientUpdatedDomainEvent(Ingredient Ingredient) : IDomainEvent;
public record IngredientSupplierBoundDomainEvent(Ingredient Ingredient) : IDomainEvent;
