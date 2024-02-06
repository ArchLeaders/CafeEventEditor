using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace CafeEventEditor.Core.Modals;

public enum LogLevel
{
    Default,
    Info,
    Debug,
    Warning,
    Error
}

public partial class AppLog : ObservableObject
{
    static AppLog()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    public static void Write(string message, LogLevel level)
    {
        Trace.WriteLine(level == LogLevel.Default ? message : $"[{level}] {message}");
    }

    public static void WriteError(Exception ex)
    {
        Trace.WriteLine($"""
            [{LogLevel.Error}] {ex.Message}
            {ex}
            """);
    }

    [ObservableProperty]
    private string _message;

    [ObservableProperty]
    private DateTime _date = DateTime.UtcNow;

    [ObservableProperty]
    private LogLevel _logLevel;

    public AppLog(string message)
    {
        int startIndex = message.IndexOf('[');
        int endIndex = message.IndexOf(']');

        if (startIndex > -1 && endIndex > -1 && Enum.TryParse(message[++startIndex..endIndex], ignoreCase: true, out LogLevel logLevel)) {
            _message = message[++endIndex..];
            _logLevel = logLevel;
        }
        else {
            _message = message;
        }
    }

    public AppLog(string message, LogLevel logLevel)
    {
        _message = message;
        _logLevel = logLevel;
    }

    public string ToMarkdown()
    {
        return $"""
            ### `[{LogLevel}]` `[{Date}]`

            ```
            {Message}
            ```
            """;
    }
}
