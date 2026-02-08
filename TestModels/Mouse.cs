using System.Text.Json.Serialization;

namespace TestModels;

public sealed partial class Mouse : InputDevice
{
    [JsonIgnore]
    public override string Type => nameof(Mouse);
}
