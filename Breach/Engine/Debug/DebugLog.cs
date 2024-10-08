using System;

namespace BraketsEngine;

public static class Debug
{
    public static void Log(string msg, object sender=null)
    {
        string message = "";
        if (sender is not null)
            message += $"[{sender}] ";

        message += msg;

        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.Gray;
    }

    public static void Warning(string msg, object sender=null)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Log("WARNING: " + msg, sender);
    }

    public static void Error(string msg, object sender=null)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Log("ERROR: " + msg, sender);
    }

    public static void Fatal(string msg, object sender=null)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Log("FATAL: " + msg, sender);

        Environment.Exit(1);
    }
}