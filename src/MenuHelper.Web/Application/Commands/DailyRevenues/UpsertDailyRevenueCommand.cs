using MenuHelper.Domain.AggregatesModel.DailyRevenueAggregate;
using MenuHelper.Infrastructure.Repositories;
namespace MenuHelper.Web.Application.Commands.DailyRevenues;

public record UpsertDailyRevenueCommand(DateOnly Date, decimal Amount, string? Note) : ICommand<DailyRevenueId>;

public class UpsertDailyRevenueCommandValidator : AbstractValidator<UpsertDailyRevenueCommand>
{
    public UpsertDailyRevenueCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithMessage("营业额不能为负数");
    }
}

public class UpsertDailyRevenueCommandHandler(IDailyRevenueRepository repository)
    : ICommandHandler<UpsertDailyRevenueCommand, DailyRevenueId>
{
    public async Task<DailyRevenueId> Handle(UpsertDailyRevenueCommand cmd, CancellationToken ct)
    {
        var existing = await repository.GetByDateAsync(cmd.Date, ct);
        if (existing is not null)
        {
            existing.Update(cmd.Amount, cmd.Note);
            return existing.Id;
        }
        var record = new DailyRevenue(cmd.Date, cmd.Amount, cmd.Note);
        await repository.AddAsync(record, ct);
        return record.Id;
    }
}
