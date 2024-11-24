namespace AdventOfCode;

internal static class ConsoleHelper
{
    internal static void WriteColored(string message, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
    {
        var previousColor = Console.ForegroundColor;
        var previousBackgroundColor = Console.BackgroundColor;
        Console.ForegroundColor = foregroundColor ?? previousColor;
        Console.BackgroundColor = backgroundColor ?? previousBackgroundColor;
        Console.Write(message);
        Console.ForegroundColor = previousColor;
        Console.BackgroundColor = previousBackgroundColor;
    }
    
    internal static void WriteLineColored(string message, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
    {
        WriteColored(message, foregroundColor, backgroundColor);
        Console.WriteLine();
    }
    
    internal static ConsoleColor TimeColor(TimeSpan time) => time switch {
        { TotalSeconds: < 1 } => ConsoleColor.Green,
        { TotalMinutes: < 1 } => ConsoleColor.Yellow,
        _ => ConsoleColor.Red
    };
}