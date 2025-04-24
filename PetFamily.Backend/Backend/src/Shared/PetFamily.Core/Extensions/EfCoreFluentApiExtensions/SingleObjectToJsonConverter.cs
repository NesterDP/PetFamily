using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PetFamily.Core.Extensions.EfCoreFluentApiExtensions;

public static class SingleObjectToJsonConverter
{
    public static PropertyBuilder<TValueObject> CustomObjectJsonConverter<TValueObject, TDto>(
        this PropertyBuilder<TValueObject> builder,
        Func<TValueObject, TDto> toDtoSelector,
        Func<TDto, TValueObject> toValueObjectSelector)
    {
        return builder.HasConversion(
                valueObject => SerializeObjectToJson(valueObject, toDtoSelector),
                json => DeserializeObjectFromJson(json, toValueObjectSelector))

                // CreateValueObjectComparer<TValueObject>())
            .HasColumnType("jsonb");
    }

    private static string SerializeObjectToJson<TValueObject, TDto>(
        TValueObject valueObject,
        Func<TValueObject, TDto> selector)
    {
        var dto = selector(valueObject);
        return JsonSerializer.Serialize(dto, JsonSerializerOptions.Default);
    }

    private static TValueObject DeserializeObjectFromJson<TValueObject, TDto>(
        string json,
        Func<TDto, TValueObject> selector)
    {
        var dto = JsonSerializer.Deserialize<TDto>(json, JsonSerializerOptions.Default);
        return selector(dto!);
    }

    /*private static ValueComparer<T> CreateValueObjectComparer<T>()
    {
        return new ValueComparer<T>(
            (c1, c2) => Serialize(c1) == Serialize(c2),
            c => Serialize(c).GetHashCode(),
            c => c);
    }*/

    /*private static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value);
    }*/

    /*private static T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json)!;
    }*/
}