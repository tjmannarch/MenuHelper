using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.DishAggregate;
using MenuHelper.Web.Application.Queries.Dishes;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Dishes;

public record ListDishesRequest(string? Keyword = null, int PageIndex = 1, int PageSize = 20);

[Tags("Dishes")]
[HttpGet("/api/dishes")]
[AllowAnonymous]
public class ListDishesEndpoint(IMediator mediator)
    : Endpoint<ListDishesRequest, ResponseData<PagedData<DishListItemDto>>>
{
    public override async Task HandleAsync(ListDishesRequest req, CancellationToken ct)
    {
        var result = await mediator.Send(new ListDishesQuery(req.Keyword, req.PageIndex, req.PageSize), ct);
        await Send.OkAsync(result.AsResponseData(), ct);
    }
}
