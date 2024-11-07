namespace Mafia_server.Command;

public interface ICommand
{
    Task ExecuteAsync();
}