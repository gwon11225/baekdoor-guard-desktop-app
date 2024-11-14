using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BackdoorGuard;

public sealed class MainWindowViewModel(ObservableCollection<OutingSchedule> outingSchedules)
    : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<OutingSchedule> OutingSchedules { get; set; } = outingSchedules;
    public String Title { get; set; } = "";
    public DateTimeOffset SelectedDate { get; set; } = DateTime.Now;
    public TimeSpan StartTime { get; set; } = TimeSpan.Zero;
    public TimeSpan EndTime { get; set; } = TimeSpan.Zero;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}