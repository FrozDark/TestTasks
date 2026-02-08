using TestModels;

namespace TestViewModels.Data;

public static partial class PersistentData
{
    public static UniversalItem[] DeviceCategories { get; } = [
        new("printer", "Принтер"),
        new("mouse", "Мышь"),
        new("keyboard", "Клавиатура"),
        new("monitor", "Монитор")
    ];

    public static DeviceStatusItem[] DeviceStatuses { get; } = [
        new(DeviceStatus.Operating, "Рабочий"),
        new(DeviceStatus.Broken, "Сломанный"),
        new(DeviceStatus.WrittenOff, "Списанный"),
    ];
}
