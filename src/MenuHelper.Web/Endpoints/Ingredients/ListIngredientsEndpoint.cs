using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using MenuHelper.Web.Application.Queries.Ingredients;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Ingredients;

public record ListIngredientsRequest(
    IngredientCategory? Category = null,
    ConsumptionType? ConsumptionType = null,
    string? Keyword = null,
    int PageIndex = 1,
    int PageSize = 20);

[Tags("Ingredients")]
[HttpGet("/api/ingredients")]
[AllowAnonymous]
public class ListIngredientsEndpoint(IMediator mediator)
    : Endpoint<ListIngredientsRequest, ResponseData<PagedData<IngredientListItemDto>>>
{
    public override async Task HandleAsync(ListIngredientsRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new ListIngredientsQuery(
            req.Category, req.ConsumptionType, req.Keyword, req.PageIndex, req.PageSize), ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}
