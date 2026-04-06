using MenuHelper.Domain.AggregatesModel.DailyRevenueAggregate;
namespace MenuHelper.Domain.DomainEvents;

public record DailyRevenueCreatedDomainEvent(DailyRevenue DailyRevenue) : IDomainEvent;
public record DailyRevenueUpdatedDomainEvent(DailyRevenue DailyRevenue) : IDomainEvent;
