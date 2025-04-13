using UnityEngine;
using System;

/// <summary>
/// A static utility class for centralized logging.
/// Provides standardized methods for logging messages, warnings, and errors,
/// including timestamps and log levels.
/// </summary>
public static class Logger
{
    /// <summary>
    /// If true, debug logs are enabled.
    /// </summary>
    public static bool IsDebugEnabled = true;

    /// <summary>
    /// Logs a standard message with a timestamp and log level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void Log(string message)
    {
        if (IsDebugEnabled)
        {
            Debug.Log(FormatMessage("LOG", message));
        }
    }

    /// <summary>
    /// Logs a warning message with a timestamp and log level.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    public static void LogWarning(string message)
    {
        Debug.LogWarning(FormatMessage("WARN", message));
    }

    /// <summary>
    /// Logs an error message with a timestamp and log level.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    public static void LogError(string message)
    {
        Debug.LogError(FormatMessage("ERROR", message));
    }

    /// <summary>
    /// Formats a log message with a timestamp and specified log level.
    /// </summary>
    /// <param name="level">The log level (e.g., "LOG", "WARN", "ERROR").</param>
    /// <param name="message">The message to format.</param>
    /// <returns>A formatted log string.</returns>
    private static string FormatMessage(string level, string message)
    {
        return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
    }
}
