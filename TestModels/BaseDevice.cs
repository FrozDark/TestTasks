using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace TestModels;

[JsonConverter(typeof(JsonStringEnumConverter<DeviceStatus>))]
public enum DeviceStatus
{
    [Description("Рабочий")]
    Operating,

    [Description("Сломанный")]
    Broken,

    [Description("Списанный")]
    WrittenOff
}

// Т.к. у нас типизированные устройства, даем понять как определять типы JSON сериализатору
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Keyboard), "keyboard")]
[JsonDerivedType(typeof(Mouse), "mouse")]
[JsonDerivedType(typeof(Printer), "printer")]
[JsonDerivedType(typeof(Monitor), "monitor")]
public abstract partial class BaseDevice : ObservableObject
{
    // По сути ненужное свойство
    [JsonIgnore]
    public abstract string Type { get; }

    // Вроде как категорию в этом проекте это явно указанный тип Mouse, Printer, Device
    //[ObservableProperty]
    //string? category;

    [ObservableProperty]
    string? _model;

    [ObservableProperty]
    string? _name;

    [ObservableProperty]
    string? _serialNumber;

    [ObservableProperty]
    DateTime _setupDate;

    [ObservableProperty]
    DeviceStatus _status;
}
