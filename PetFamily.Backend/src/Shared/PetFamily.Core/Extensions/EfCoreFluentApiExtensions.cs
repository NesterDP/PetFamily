using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PetFamily.Core.Extensions;

public static class EfCoreFluentApiExtensions
{ 
    public static PropertyBuilder<IReadOnlyList<TValueObject>> CustomJsonCollectionConverter<TValueObject, TDto>(
        this PropertyBuilder<IReadOnlyList<TValueObject>> builder,
        Func<TValueObject, TDto> toDtoSelector,
        Func<TDto, TValueObject> toValueObjectSelector)
    {
        return builder.HasConversion(
                valueObjects => SerializeVoCollection(valueObjects, toDtoSelector),
                json => DeserializeDtoCollection(json, toValueObjectSelector),
                CreateCollectionValueComparer<TValueObject>())
            .HasColumnType("jsonb");
    }

    private static string SerializeVoCollection<TValueObject, TDto>(
        IReadOnlyList<TValueObject> valueObjects,
        Func<TValueObject, TDto> selector)
    {
        var dtos = valueObjects.Select(selector);
        
        return JsonSerializer.Serialize(dtos, JsonSerializerOptions.Default);
    }

    private static IReadOnlyList<TValueObject> DeserializeDtoCollection<TValueObject, TDto>(
        string json, Func<TDto, TValueObject> selector)
    {
        var dtos = JsonSerializer.Deserialize<IEnumerable<TDto>>(json, JsonSerializerOptions.Default) ?? [];

        return dtos.Select(selector).ToList();
    }
    
    private static ValueComparer<IReadOnlyList<T>> CreateCollectionValueComparer<T>()
    {
        return new ValueComparer<IReadOnlyList<T>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())),
            c => c.ToList());
    }
    
    public static PropertyBuilder<List<TValueObject>> CustomListJsonCollectionConverter<TValueObject, TDto>(
        this PropertyBuilder<List<TValueObject>> builder,
        Func<TValueObject, TDto> toDtoSelector,
        Func<TDto, TValueObject> toValueObjectSelector)
    {
        return builder.HasConversion(
                valueObjects => SerializeVoCollection(valueObjects, toDtoSelector),
                json => DeserializeListDtoCollection(json, toValueObjectSelector),
                CreateListCollectionValueComparer<TValueObject>())
            .HasColumnType("jsonb");
    }

    private static string SerializeVoCollection<TValueObject, TDto>(
        List<TValueObject> valueObjects,
        Func<TValueObject, TDto> selector)
    {
        var dtos = valueObjects.Select(selector);
        
        return JsonSerializer.Serialize(dtos, JsonSerializerOptions.Default);
    }

    private static List<TValueObject> DeserializeListDtoCollection<TValueObject, TDto>(
        string json, Func<TDto, TValueObject> selector)
    {
        var dtos = JsonSerializer.Deserialize<IEnumerable<TDto>>(json, JsonSerializerOptions.Default) ?? [];

        return dtos.Select(selector).ToList();
    }
    
    private static ValueComparer<List<T>> CreateListCollectionValueComparer<T>()
    {
        return new ValueComparer<List<T>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())),
            c => c.ToList());
    }
}   