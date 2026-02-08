using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using TestModels;

namespace TestViewModels.Json;

/// <summary>
/// Избегаем рефлексию используя соурс генератор
/// </summary>
[JsonSourceGenerationOptions(IncludeFields = false, UseStringEnumConverter = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(BaseDevice))]
[JsonSerializable(typeof(ObservableCollection<BaseDevice>))]
public sealed partial class ModelsSerializationContext : JsonSerializerContext
{
}
