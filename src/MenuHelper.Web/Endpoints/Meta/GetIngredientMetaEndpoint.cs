using System.ComponentModel;
using System.Reflection;
using FastEndpoints;
using MenuHelper.Domain.AggregatesModel.IngredientAggregate;
using Microsoft.AspNetCore.Authorization;

namespace MenuHelper.Web.Endpoints.Meta;

/// <summary>枚举选项 DTO</summary>
public record EnumOptionDto(int Value, string Label);

/// <summary>原材料元数据响应</summary>
public record IngredientMetaResponse(
    List<EnumOptionDto> Categories,
    List<EnumOptionDto> ConsumptionTypes);

[Tags("Meta")]
[HttpGet("/api/meta/ingredients")]
[AllowAnonymous]
public class GetIngredientMetaEndpoint : EndpointWithoutRequest<ResponseData<IngredientMetaResponse>>
{
    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = new IngredientMetaResponse(
            GetEnumOptions<IngredientCategory>(),
            GetEnumOptions<ConsumptionType>()
        );
        await Send.OkAsync(response.AsResponseData(), ct);
    }

    /// <summary>通过反射读取枚举值及其 Description 标注，生成选项列表</summary>
    private static List<EnumOptionDto> GetEnumOptions<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(e =>
            {
                var field = typeof(TEnum).GetField(e.ToString())!;
                var label = field.GetCustomAttribute<DescriptionAttribute>()?.Description ?? e.ToString();
                return new EnumOptionDto(Convert.ToInt32(e), label);
            })
            .ToList();
    }
}
