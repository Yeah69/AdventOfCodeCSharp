using System;
using System.Collections.Immutable;
using System.Linq;
using AdventOfCode.Days;
using AdventOfCode.Days.Pickers;

namespace AdventOfCode.View.ViewModels;

internal sealed class DaysViewModel : ViewModelBase
{
    public DaysViewModel(
        ImmutableArray<IDay> days,
        PickAllWithSamples picker,
        Func<IDay, DayViewModel> dayViewModelFactory)
    {
        Days = [..picker.PickDays(days).Select(dayViewModelFactory)];
        SelectedDay = Days[^1];
    }

    internal ImmutableArray<DayViewModel> Days { get; }

    internal DayViewModel SelectedDay
    {
        get;
        set => SetField(ref field, value);
    }
}