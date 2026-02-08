using System.Text.Json.Serialization;

namespace TestModels;

public sealed partial class Keyboard : InputDevice
{
    [JsonIgnore]
    public override string Type => nameof(Keyboard);
}
