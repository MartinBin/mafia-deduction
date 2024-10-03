using System;

namespace Mafia_server
{
    public static class Logger
    {
        private static ConsoleColor defaultColor;

        public static void Initialize(ConsoleColor color)
        {
            defaultColor = color;
            Console.ForegroundColor = defaultColor;
        }

        public static void Log(LogType type, string message)
        {
            switch (type)
            {
                case LogType.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogType.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogType.Debug:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
            }

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {type}: {message}");
            Console.ForegroundColor = defaultColor;
        }
    }
}