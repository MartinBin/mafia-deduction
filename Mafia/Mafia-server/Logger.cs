using System;
using System.IO;

namespace Mafia_server
{
    public sealed class Logger
    {
        private static readonly Lazy<Logger> lazy = new Lazy<Logger>(() => new Logger());

        private string logFolderPath;
        private static ConsoleColor defaultColor;

        private Logger()
        {
            logFolderPath = AppDomain.CurrentDomain.BaseDirectory;
            Initialize(ConsoleColor.White);
        }
        
        public void SetLogFolderPath(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                logFolderPath = folderPath;
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(folderPath);
                    logFolderPath = folderPath;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to set log folder path: {ex.Message}");
                }
            }
        }
        
        public static Logger getInstance
        {
            get
            {
                return lazy.Value;
            }
        }
        
        public static void Initialize(ConsoleColor color)
        {
            defaultColor = color;
            Console.ForegroundColor = defaultColor;
        }

        public void Log(LogType type, string message)
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

            string logMessage = $"[{DateTime.Now:HH:mm:ss}] {type}: {message}";
            
            Console.WriteLine(logMessage);
            Console.ForegroundColor = defaultColor;
            
            WriteLogToFile(logMessage);
        }
        
        private string GetLogFilePath()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            return Path.Combine(logFolderPath, $"log_{date}.txt");
        }
        
        private void WriteLogToFile(string message)
        {
            string logFilePath = GetLogFilePath();
            try
            {
                File.AppendAllText(logFilePath, message + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
    }
}