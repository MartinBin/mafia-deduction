namespace Mafia_server.Log
{
    public abstract class LogHandler
    {
        protected LogHandler nextHandler;

        public void SetNext(LogHandler handler)
        {
            nextHandler = handler;
        }

        public abstract void HandleLog(LogType type, string message);
    }

}
