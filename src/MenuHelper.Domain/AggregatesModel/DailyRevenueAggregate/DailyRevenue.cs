using MenuHelper.Domain.DomainEvents;
namespace MenuHelper.Domain.AggregatesModel.DailyRevenueAggregate;

public partial record DailyRevenueId : IGuidStronglyTypedId;

/// <summary>每日营业收入记录（按日唯一）</summary>
public class DailyRevenue : Entity<DailyRevenueId>, IAggregateRoot
{
    protected DailyRevenue() { }

    public DailyRevenue(DateOnly date, decimal amount, string? note = null)
    {
        if (amount < 0) throw new KnownException("营业额不能为负数");
        Date = date;
        Amount = amount;
        Note = note;
        this.AddDomainEvent(new DailyRevenueCreatedDomainEvent(this));
    }

    /// <summary>营业日期（每日唯一）</summary>
    public DateOnly Date { get; private set; }
    /// <summary>营业额（元）</summary>
    public decimal Amount { get; private set; }
    /// <summary>备注</summary>
    public string? Note { get; private set; }
    public Deleted Deleted { get; private set; } = new();
    public RowVersion RowVersion { get; private set; } = new(0);

    public void Update(decimal amount, string? note)
    {
        if (amount < 0) throw new KnownException("营业额不能为负数");
        Amount = amount;
        Note = note;
        this.AddDomainEvent(new DailyRevenueUpdatedDomainEvent(this));
    }
}
