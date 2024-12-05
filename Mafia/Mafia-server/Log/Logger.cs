using System;
using System.IO;

namespace Mafia_server.Log
{
    public sealed class Logger
    {
        private static readonly Lazy<Logger> lazy = new Lazy<Logger>(() => new Logger());

        private LogHandler firstHandler;

        private Logger()
        {

        }


        public static Logger getInstance
        {
            get
            {
                return lazy.Value;
            }
        }

        public void SetHandlerChain(LogHandler handler)
        {
            firstHandler = handler;
        }

        public void Log(LogType type, string message)
        {
            firstHandler?.HandleLog(type, message);
        }
    }
}