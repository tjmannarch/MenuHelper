using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;

namespace MenuHelper.Domain.DomainEvents;

public record DishCreatedDomainEvent(Dish Dish) : IDomainEvent;
public record DishIngredientAddedDomainEvent(Dish Dish, IngredientId IngredientId) : IDomainEvent;
public record DishIngredientRemovedDomainEvent(Dish Dish, IngredientId IngredientId) : IDomainEvent;
