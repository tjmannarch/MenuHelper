using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Domain.AggregatesModel.InventoryCheckAggregate;
using MenuHelper.Domain.DomainEvents;

namespace MenuHelper.Domain.Tests;

public class InventoryCheckTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateInventoryCheck()
    {
        // Arrange
        var checkDate = new DateOnly(2026, 4, 1);
        var ingredientId = new IngredientId(Guid.NewGuid());
        var items = new[] { (ingredientId, 10.5m) };

        // Act
        var inventoryCheck = new InventoryCheck(checkDate, items);

        // Assert
        Assert.Equal(checkDate, inventoryCheck.CheckDate);
        Assert.Single(inventoryCheck.Items);
        Assert.Equal(ingredientId, inventoryCheck.Items[0].IngredientId);
        Assert.Equal(10.5m, inventoryCheck.Items[0].Quantity);
        Assert.False(inventoryCheck.Deleted);
    }

    [Fact]
    public void Constructor_ShouldPublishInventoryCheckCreatedDomainEvent()
    {
        // Arrange
        var checkDate = new DateOnly(2026, 4, 1);
        var items = new[] { (new IngredientId(Guid.NewGuid()), 5m) };

        // Act
        var inventoryCheck = new InventoryCheck(checkDate, items);

        // Assert
        var domainEvents = inventoryCheck.GetDomainEvents();
        Assert.Single(domainEvents);
        var evt = Assert.IsType<InventoryCheckCreatedDomainEvent>(domainEvents.First());
        Assert.Equal(inventoryCheck, evt.InventoryCheck);
    }

    [Fact]
    public void Constructor_WithEmptyItems_ShouldCreateInventoryCheckWithNoItems()
    {
        // Arrange
        var checkDate = new DateOnly(2026, 4, 1);

        // Act
        var inventoryCheck = new InventoryCheck(checkDate, []);

        // Assert
        Assert.Equal(checkDate, inventoryCheck.CheckDate);
        Assert.Empty(inventoryCheck.Items);
    }

    [Fact]
    public void Constructor_WithDuplicateIngredientId_ShouldThrowKnownException()
    {
        // Arrange
        var checkDate = new DateOnly(2026, 4, 1);
        var ingredientId = new IngredientId(Guid.NewGuid());
        var items = new[] { (ingredientId, 5m), (ingredientId, 10m) };

        // Act & Assert
        var ex = Assert.Throws<KnownException>(() => new InventoryCheck(checkDate, items));
        Assert.Equal("同一盘点中不允许重复录入同一原材料", ex.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.001)]
    public void Constructor_WithNegativeQuantity_ShouldThrowKnownException(decimal quantity)
    {
        // Arrange
        var checkDate = new DateOnly(2026, 4, 1);
        var items = new[] { (new IngredientId(Guid.NewGuid()), quantity) };

        // Act & Assert
        var ex = Assert.Throws<KnownException>(() => new InventoryCheck(checkDate, items));
        Assert.Equal("库存数量不能小于0", ex.Message);
    }

    [Fact]
    public void Constructor_WithZeroQuantity_ShouldSucceed()
    {
        // Arrange
        var checkDate = new DateOnly(2026, 4, 1);
        var ingredientId = new IngredientId(Guid.NewGuid());
        var items = new[] { (ingredientId, 0m) };

        // Act
        var inventoryCheck = new InventoryCheck(checkDate, items);

        // Assert
        Assert.Single(inventoryCheck.Items);
        Assert.Equal(0m, inventoryCheck.Items[0].Quantity);
    }

    [Fact]
    public void SetItems_ShouldReplaceAllExistingItems()
    {
        // Arrange
        var checkDate = new DateOnly(2026, 4, 1);
        var id1 = new IngredientId(Guid.NewGuid());
        var id2 = new IngredientId(Guid.NewGuid());
        var id3 = new IngredientId(Guid.NewGuid());
        var inventoryCheck = new InventoryCheck(checkDate, new[] { (id1, 5m), (id2, 10m) });
        inventoryCheck.ClearDomainEvents();

        var newItems = new[] { (id2, 20m), (id3, 15m) };

        // Act
        inventoryCheck.SetItems(newItems);

        // Assert
        Assert.Equal(2, inventoryCheck.Items.Count);
        Assert.DoesNotContain(inventoryCheck.Items, x => x.IngredientId == id1);
        Assert.Contains(inventoryCheck.Items, x => x.IngredientId == id2 && x.Quantity == 20m);
        Assert.Contains(inventoryCheck.Items, x => x.IngredientId == id3 && x.Quantity == 15m);
    }

    [Fact]
    public void SetItems_ShouldPublishInventoryCheckItemsUpdatedDomainEvent()
    {
        // Arrange
        var checkDate = new DateOnly(2026, 4, 1);
        var inventoryCheck = new InventoryCheck(checkDate, []);
        inventoryCheck.ClearDomainEvents();

        var newItems = new[] { (new IngredientId(Guid.NewGuid()), 8m) };

        // Act
        inventoryCheck.SetItems(newItems);

        // Assert
        var domainEvents = inventoryCheck.GetDomainEvents();
        Assert.Single(domainEvents);
        var evt = Assert.IsType<InventoryCheckItemsUpdatedDomainEvent>(domainEvents.First());
        Assert.Equal(inventoryCheck, evt.InventoryCheck);
    }

    [Fact]
    public void SetItems_WithDuplicateIngredientId_ShouldThrowKnownException()
    {
        // Arrange
        var inventoryCheck = new InventoryCheck(new DateOnly(2026, 4, 1), []);
        var ingredientId = new IngredientId(Guid.NewGuid());
        var items = new[] { (ingredientId, 5m), (ingredientId, 10m) };

        // Act & Assert
        var ex = Assert.Throws<KnownException>(() => inventoryCheck.SetItems(items));
        Assert.Equal("同一盘点中不允许重复录入同一原材料", ex.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.5)]
    public void SetItems_WithNegativeQuantity_ShouldThrowKnownException(decimal quantity)
    {
        // Arrange
        var inventoryCheck = new InventoryCheck(new DateOnly(2026, 4, 1), []);
        var items = new[] { (new IngredientId(Guid.NewGuid()), quantity) };

        // Act & Assert
        var ex = Assert.Throws<KnownException>(() => inventoryCheck.SetItems(items));
        Assert.Equal("库存数量不能小于0", ex.Message);
    }

    [Fact]
    public void SetItems_WithEmptyList_ShouldClearAllItems()
    {
        // Arrange
        var id1 = new IngredientId(Guid.NewGuid());
        var inventoryCheck = new InventoryCheck(new DateOnly(2026, 4, 1), new[] { (id1, 5m) });

        // Act
        inventoryCheck.SetItems([]);

        // Assert
        Assert.Empty(inventoryCheck.Items);
    }

    [Fact]
    public void SetItems_OriginalItemsNotMutatedOnValidationFailure()
    {
        // Arrange
        var id1 = new IngredientId(Guid.NewGuid());
        var id2 = new IngredientId(Guid.NewGuid());
        var inventoryCheck = new InventoryCheck(new DateOnly(2026, 4, 1), new[] { (id1, 5m) });

        // Act - pass duplicate to trigger error
        Assert.Throws<KnownException>(() => inventoryCheck.SetItems(new[] { (id2, 1m), (id2, 2m) }));

        // Assert - original items should be unchanged
        Assert.Single(inventoryCheck.Items);
        Assert.Equal(id1, inventoryCheck.Items[0].IngredientId);
    }

    [Fact]
    public void MultipleSetItems_ShouldReflectLastCallOnly()
    {
        // Arrange
        var id1 = new IngredientId(Guid.NewGuid());
        var id2 = new IngredientId(Guid.NewGuid());
        var id3 = new IngredientId(Guid.NewGuid());
        var inventoryCheck = new InventoryCheck(new DateOnly(2026, 4, 1), new[] { (id1, 5m) });

        // Act - simulate multiple modifications on the same day
        inventoryCheck.SetItems(new[] { (id1, 10m), (id2, 20m) });
        inventoryCheck.SetItems(new[] { (id3, 30m) });

        // Assert - only last call remains
        Assert.Single(inventoryCheck.Items);
        Assert.Equal(id3, inventoryCheck.Items[0].IngredientId);
        Assert.Equal(30m, inventoryCheck.Items[0].Quantity);
    }
}
