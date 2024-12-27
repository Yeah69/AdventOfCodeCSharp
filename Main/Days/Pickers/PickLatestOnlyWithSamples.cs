﻿namespace AdventOfCode.Days.Pickers;

internal class PickLatestOnlyWithSamples : IDayPicker
{
    public IEnumerable<IDay> PickDay(IReadOnlyList<IDay> days)
    {
        if (days.Where(d => !string.IsNullOrEmpty(d.Input))
                .OrderByDescending(d => d.Number)
                .FirstOrDefault() is { } latest)
        {
            foreach (var picked in latest.Samples().Append(latest))
            {
                yield return picked;
            }
        }
    }
}