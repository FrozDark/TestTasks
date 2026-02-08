using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;
using TestModels;
using TestViewModels.Data;
using Monitor = TestModels.Monitor;

namespace TestViewModels.Dialogs;

public sealed partial class AddDeviceDialogViewModel : ResultDialogViewModelBase<BaseDevice?>
{
    [ObservableProperty]
    public string? _category;

    [ObservableProperty]
    [Required, StringLength(255, MinimumLength = 1), MinLength(1)]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    string? _model;

    [ObservableProperty]
    [Required, StringLength(255, MinimumLength = 1), MinLength(1)]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    string? _name;

    [ObservableProperty]
    [Required, StringLength(255, MinimumLength = 1), MinLength(1)]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    string? _serialNumber;

    [ObservableProperty]
    DateTime _setupDate = DateTime.Now;

    [ObservableProperty]
    DeviceStatus _status = DeviceStatus.Operating;

    internal AddDeviceDialogViewModel()
    {
        Category = Categories[0].Name;
    }

    public DeviceStatusItem[] Statuses => PersistentData.DeviceStatuses;

    public UniversalItem[] Categories => PersistentData.DeviceCategories;

    [RelayCommand]
    void Cancel()
    {
        Dismiss(null);
    }

    public bool CanConfirm()
    {
        return !string.IsNullOrEmpty(Model)
            && !string.IsNullOrEmpty(SerialNumber)
            && !string.IsNullOrEmpty(Name);
    }

    [RelayCommand(CanExecute = nameof(CanConfirm))]
    void Confirm()
    {
        BaseDevice? _result = null;

        switch (Category)
        {
            case "keyboard":
                _result = new Keyboard();
                break;
            case "mouse":
                _result = new Mouse();
                break;
            case "printer":
                _result = new Printer();
                break;
            case "monitor":
                _result = new Monitor();
                break;
        }

        if (_result is not null)
        {
            _result.Model = Model;
            _result.Name = Name;
            _result.SerialNumber = SerialNumber;
            _result.Status = Status;
            _result.SetupDate = SetupDate;
        }

        Dismiss(_result);
    }
}
