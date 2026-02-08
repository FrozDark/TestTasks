using System.Text.Json.Serialization;

namespace TestModels;

public sealed partial class Printer : OutputDevice
{
    [JsonIgnore]
    public override string Type => nameof(Printer);
}
