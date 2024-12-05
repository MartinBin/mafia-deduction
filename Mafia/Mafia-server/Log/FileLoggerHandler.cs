namespace Mafia_server.Log
{
    public class FileLoggerHandler : LogHandler
    {
        private string logFolderPath;

        public FileLoggerHandler(string folderPath)
        {
            logFolderPath = folderPath;
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }
        }

        public override void HandleLog(LogType type, string message)
        {
            string logFilePath = Path.Combine(logFolderPath, $"log_{DateTime.Now:yyyy-MM-dd}.txt");
            string logMessage = $"[{DateTime.Now:HH:mm:ss}] {type}: {message}";

            try
            {
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }

            nextHandler?.HandleLog(type, message);
        }
    }
}
