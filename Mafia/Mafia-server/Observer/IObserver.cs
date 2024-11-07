namespace Mafia_server.Observer;

public interface IObserver
{
    Task UpdateAsync(string command, params string[] parameters);
}