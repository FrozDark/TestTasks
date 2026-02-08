using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TestModels.Examples;

// Ну чисто демонстрация о том, что можно сделать свою реализацию событий свойств
public partial class ExampleSelfDecribed : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string? ExampleStringProperty
    {
        get => field; // Спасибо новому C# 14, не нужно заниматься шаблонным кодом
        set => SetProeprty(ref field, value);
    }

    protected bool SetProeprty<T>(ref T value, T newVal, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(value, newVal))
        {
            value = newVal;
            OnPropertyChanged(propertyName);

            return true;
        }

        return false;
    }

    protected virtual void OnPropertyChanged(string? propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
