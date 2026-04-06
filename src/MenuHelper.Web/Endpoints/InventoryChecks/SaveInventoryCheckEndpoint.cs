using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Web.Application.Commands.InventoryChecks;
using MenuHelper.Domain.AggregatesModel.InventoryCheckAggregate;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.InventoryChecks;

public record SaveInventoryCheckRequestDto(List<SaveInventoryCheckItemDto> Items);

public record SaveInventoryCheckItemDto(Guid IngredientId, decimal Quantity);

public record SaveInventoryCheckResponseDto(InventoryCheckId InventoryCheckId);

[Tags("InventoryChecks")]
[HttpPut("/api/inventory-checks/{date}")]
[AllowAnonymous]
public class SaveInventoryCheckEndpoint(IMediator mediator)
    : Endpoint<SaveInventoryCheckRequestDto, ResponseData<SaveInventoryCheckResponseDto>>
{
    public override async Task HandleAsync(SaveInventoryCheckRequestDto req, CancellationToken ct)
    {
        var dateStr = Route<string>("date")!;
        var checkDate = DateOnly.Parse(dateStr, System.Globalization.CultureInfo.InvariantCulture);

        var items = req.Items
            .Select(x => new InventoryCheckItemInput(new IngredientId(x.IngredientId), x.Quantity))
            .ToList();

        var command = new SaveInventoryCheckCommand(checkDate, items);
        var id = await mediator.Send(command, ct);

        await Send.OkAsync(new SaveInventoryCheckResponseDto(id).AsResponseData(), ct);
    }
}
