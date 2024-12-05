namespace Mafia_server.Log
{
    public class ConsoleLoggerHandler : LogHandler
    {
        public override void HandleLog(LogType type, string message)
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
            Console.ResetColor();

            nextHandler?.HandleLog(type, message);
        }
    }
}
