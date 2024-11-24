namespace AdventOfCode.Days;

internal static class DayHelper
{
    internal static void WriteDayLabel(IDay day, string partLabel)
    {
        Console.Write("Day ");
        ConsoleHelper.WriteColored(day.Number.ToString().PadLeft(2, '0'), ConsoleColor.DarkBlue);
        Console.Write(" ");
        ConsoleHelper.WriteColored(partLabel, ConsoleColor.Magenta);
        if (!string.IsNullOrWhiteSpace(partLabel))
            Console.Write(" Part");
    }
}