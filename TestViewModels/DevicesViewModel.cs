using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;
using TestModels;
using TestViewModels.Data;
using TestViewModels.Dialogs;
using TestViewModels.Interfaces;
using TestViewModels.Json;
using Monitor = TestModels.Monitor;

namespace TestViewModels;

public sealed partial class DevicesViewModel : PageViewModelBase, IPageInstance<DevicesViewModel>
{
    [ObservableProperty]
    string? _searchText;

    // Не все UI фреймворки позволяют оборачивать enum в какой-нибудь класс.
    // Но WPF это может делать
    [ObservableProperty]
    DeviceStatus? _searchStatus;

    [ObservableProperty]
    BaseDevice? _selectedDevice;

    // Все устройства будут помещаться сюда
    readonly ObservableCollection<BaseDevice> _allDevices = [];

    // Сюда будут помещаться фильтрованные устройства
    readonly ObservableCollection<BaseDevice> _filteredDevices = [];

    readonly RandomDateTime _randomDateTime = new();

    public DevicePropertiesViewModel DeviceProperties { get; }

    // Где будем хранить данные
    const string _jsonFileName = "data.json";

    // В идеале, мы не должны позволять создавать любые ViewModel извне проекта, вся работа должна вестись изнутри.
    // Поэтому контруктор internal
    // Например из MainViewModel или другие навигационные вью-модели, которых в этом проекте нет
    internal DevicesViewModel()
    {
        DeviceProperties = new(this);

        Statuses = [new(null, "Без статуса"),.. PersistentData.DeviceStatuses];

        if (!File.Exists(_jsonFileName))
        {
            // Я не мастер придумывать названия
            _allDevices.Add(new Keyboard()
            {
                Model = "Genius",
                Name = "Клавиатура механическая Genius",
                SerialNumber = Guid.NewGuid().ToString(),
                SetupDate = _randomDateTime.Next(),
                Status = DeviceStatus.Operating // По умолчанию: рабочий
            });

            _allDevices.Add(new Keyboard()
            {
                Model = "Raptor X8",
                Name = "Клавиатура беспроводная",
                SerialNumber = Guid.NewGuid().ToString(),
                SetupDate = _randomDateTime.Next(),
                Status = DeviceStatus.Broken
            });

            _allDevices.Add(new Keyboard()
            {
                Model = "Leaven",
                Name = "Клавиатура маленькая",
                SerialNumber = Guid.NewGuid().ToString(),
                SetupDate = _randomDateTime.Next(),
                Status = DeviceStatus.WrittenOff
            });

            _allDevices.Add(new Printer()
            {
                Model = "HP Q8S",
                Name = "Принтер многофункциональный",
                SerialNumber = Guid.NewGuid().ToString(),
                SetupDate = _randomDateTime.Next(),
                Status = DeviceStatus.Broken
            });

            _allDevices.Add(new Printer()
            {
                Model = "Samsung BS8",
                Name = "Принтер многофункциональный",
                SerialNumber = Guid.NewGuid().ToString(),
                SetupDate = _randomDateTime.Next(),
                Status = DeviceStatus.Operating
            });

            _allDevices.Add(new Printer()
            {
                Model = "Xiaomi BS8",
                Name = "Принтер многофункциональный",
                SerialNumber = Guid.NewGuid().ToString(),
                SetupDate = _randomDateTime.Next(),
                Status = DeviceStatus.WrittenOff
            });

            // Данный монитор попадёт в категорию Устройства вывода, т.к. нету шаблона специально для монитора,
            // но есть для устройств наследующие от OutputDevice, в данном случае Monitor
            _allDevices.Add(new Monitor()
            {
                Model = "Samsung Odyssey",
                Name = "Монитор игровой 120Гц",
                SerialNumber = Guid.NewGuid().ToString(),
                SetupDate = _randomDateTime.Next(),
                Status = DeviceStatus.WrittenOff
            });

            StoreData();
        }
        else
        {
            using (var file = File.Open(_jsonFileName, FileMode.Open, FileAccess.Read))
            {
                _allDevices = JsonSerializer.Deserialize(file, ModelsSerializationContext.Default.ObservableCollectionBaseDevice)!;
            }
        }

        Items = _allDevices;
    }

    // Из-за ограничения того, что наш ViewModels не имеет отношения к UI и не имеет контрактов/интерфейсов от WPF, то делаем фильтр/поиск тут
    // Данное свойство будет ссылаться на ту или иную коллекцию в зависимости от контекста (в нашем случае, поиска)
    // А можно было использовать всегда коллекцию FilteredItems и просто заполнять и очищать
    public IList<BaseDevice> Items
    {
        get => field; // Возможно ключевое слово field будет ругаться, но проект будет компилироваться. Потребуется студия 2026
        private set => SetProperty(ref field, value);
    }

    // Гораздо эффективнее использовать массив.
    // Вынес в статику
    public DeviceStatusItem[] Statuses { get; } = [new(null, "Без статуса"), .. PersistentData.DeviceStatuses];

    /// <summary>
    /// Авто-генерация команды по методу
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    public async Task AddDevice()
    {
        var result = await Dialogs!.ShowDialog(new AddDeviceDialogViewModel());

        if (result.IsSuccess
            && result.Value is { } newDevice)
        {
            _allDevices.Add(newDevice);

            // Не забываем про наш поиск.
            // Ну а по-хорошему можно подписаться на CollectionChanged у _allDevices и там контролировать поиск
            if ((!string.IsNullOrEmpty(SearchText) || SearchStatus is not null)
                && IsMeetsSearchPattern(newDevice))
            {
                _filteredDevices.Add(newDevice);
            }

            StoreData();
        }
    }

    [RelayCommand]
    public async Task DeleteDeviceAsync(BaseDevice device)
    {
        var result = await Dialogs!.ShowDialog(new ConfirmDialogViewModel("Подтвердите действие"
                                    , $"Вы уверены что хотите удалить устройство {device.Name}?")
        {
            HasCloseButton = false,
            HasCancelButton = true,
            HasConfirmButton = true,
        });

        // Не смущайтесь nullable ?? чеку. Хоть диалоговое окно, гарантировано возвращает результат,
        // всё же лучше дополнительно удостовериться, чем ловить исключения NRE
        if (result.IsSuccess && (result.Value ?? false))
        {
            _allDevices.Remove(device);
            _filteredDevices.Remove(device); // Если вдруг он там есть

            StoreData();
        }
    }

    partial void OnSearchTextChanged(string? value)
    {
        RefillFilter();
    }

    partial void OnSearchStatusChanged(DeviceStatus? value)
    {
        RefillFilter();
    }

    void RefillFilter()
    {
        if (string.IsNullOrEmpty(SearchText)
            && SearchStatus is null)
        {
            // Можно было бы всегда использовать _filteredDevices, просто меняя или заполняя его
            Items = _allDevices;
        }
        else
        {
            // Если коллекция слишком огромная, то лучше создать новый объект чем очищать и заполнять... Лучше производительность 
            _filteredDevices.Clear();
            foreach (var item in _allDevices.Where(IsMeetsSearchPattern))
            {
                _filteredDevices.Add(item);
            }
            Items = _filteredDevices;
        }
    }

    bool IsMeetsSearchPattern(BaseDevice device)
    {
        if (SearchStatus is { } status
            && device.Status != status)
        {
            return false;
        }

        if (string.IsNullOrEmpty(SearchText))
        {
            return true;
        }

        // Не должно быть null у названия и модели, доверяем себе
        return device.Name!.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)
            || device.Model!.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase); // Деаем поиск по модели тоже
    }

    /// <summary>
    /// Примитивно сохраняем в файл
    /// </summary>
    internal void StoreData()
    {
        // Сериализуем напрямую без аллокации string
        using (var file = File.Open(_jsonFileName, FileMode.Create, FileAccess.Write))
        using (var writer = new Utf8JsonWriter(file))
        {
            JsonSerializer.Serialize(writer, _allDevices, ModelsSerializationContext.Default.ObservableCollectionBaseDevice);
        }
    }

    partial void OnSelectedDeviceChanged(BaseDevice? value)
    {
        DeviceProperties.SetupDevice(value);
    }

    static DevicesViewModel IPageInstance<DevicesViewModel>.CreateInstance(NavigationPageViewModelBase navigation)
    {
        return new DevicesViewModel();
    }
}

// Загугленный класс генерации рандомной даты
class RandomDateTime
{
    readonly DateTime _start = new DateTime(2015, 1, 1);
    readonly Random _random = new Random();
    readonly int _range;

    public RandomDateTime()
    {
        _range = (DateTime.Today - _start).Days;
    }

    public DateTime Next()
    {
        return _start.AddDays(_random.Next(_range)).AddHours(_random.Next(0, 24)).AddMinutes(_random.Next(0, 60)).AddSeconds(_random.Next(0, 60));
    }
}
