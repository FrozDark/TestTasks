using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using TestModels;
using TestViewModels.Data;

namespace TestViewModels;

public sealed partial class DevicePropertiesViewModel(DevicesViewModel devicesViewModel) : ViewModelBase
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand), nameof(RestoreCommand))]
    string? _model;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand), nameof(RestoreCommand))]
    string? _name;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand), nameof(RestoreCommand))]
    string? _serialNumber;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand), nameof(RestoreCommand))]
    DateTime _setupDate;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand), nameof(RestoreCommand))]
    DeviceStatus _status;

    public DeviceStatusItem[] Statuses => PersistentData.DeviceStatuses;

    // Категория не может меняться. Ведь принтер не может вдруг стать клавиатурой
    // Поэтому строкое представление и только чтение
    public string? Category
    {
        get => field;
        private set => SetProperty(ref field, value);
    }

    public BaseDevice? CurrentDevice
    {
        get => field;
        private set
        {
            if (!EqualityComparer<BaseDevice>.Default.Equals(field, value))
            {
                var oldValue = field;
                OnPropertyChanging();
                field = value;
                OnPropertyChanged();

                OnCurrentDeviceChanged(oldValue, value);
            }
        }
    }

    internal void SetupDevice(BaseDevice? device)
    {
        CurrentDevice = device;
    }

    public bool CanStore()
    {
        if (CurrentDevice is null)
        {
            return false;
        }
        return !string.Equals(Model, CurrentDevice.Model, StringComparison.Ordinal)
            || !string.Equals(Name, CurrentDevice.Name, StringComparison.Ordinal)
            || !string.Equals(SerialNumber, CurrentDevice.SerialNumber, StringComparison.Ordinal)
            || SetupDate != CurrentDevice.SetupDate
            || Status != CurrentDevice.Status;
    }

    [RelayCommand(CanExecute = nameof(CanStore))]
    public void Save()
    {
        if (CanStore())
        {
            CurrentDevice!.Model = Model;
            CurrentDevice!.Name = Name;
            CurrentDevice!.SerialNumber = SerialNumber;
            CurrentDevice!.SetupDate = SetupDate;
            CurrentDevice!.Status = Status;
            devicesViewModel.StoreData();
        }
    }

    // Можно было оставить только CanStore
    public bool CanRestore()
    {
        return CanStore();
    }

    [RelayCommand(CanExecute = nameof(CanRestore))]
    public void Restore()
    {
        if (CanRestore())
        {
            InheritProperties(CurrentDevice!);
        }
    }

    void OnCurrentDeviceChanged(BaseDevice? oldDevice, BaseDevice? newDevice)
    {
        if (oldDevice is not null)
        {
            oldDevice.PropertyChanged -= OnDevicePropertyChanged;
        }

        if (newDevice is not null)
        {
            Category = PersistentData.DeviceCategories.First(c => c.Name.Equals(newDevice.Type, StringComparison.OrdinalIgnoreCase)).Display;
            InheritProperties(newDevice);

            newDevice.PropertyChanged += OnDevicePropertyChanged;
        }
        else
        {
            ResetProperties();
        }

        NotifyCommandsCanExecute();
    }

    void InheritProperties(BaseDevice device)
    {
        Model = device.Model;
        Name = device.Name;
        SerialNumber = device.SerialNumber;
        SetupDate = device.SetupDate;
        Status = device.Status;
    }

    void ResetProperties()
    {
        Category = null;
        Model = null;
        Name = null;
        SerialNumber = null;
    }

    void OnDevicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        NotifyCommandsCanExecute();
    }

    void NotifyCommandsCanExecute()
    {
        RestoreCommand.NotifyCanExecuteChanged();
        SaveCommand.NotifyCanExecuteChanged();
    }
}
