namespace Mafia_server.Observer;

public class CommandSubject
{
    private readonly List<IObserver> _observers = new List<IObserver>();

    public void RegisterObserver(IObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void UnregisterObserver(IObserver observer)
    {
        if (_observers.Contains(observer))
        {
            _observers.Remove(observer);
        }
    }

    public async Task NotifyObserversAsync(string command, params string[] parameters)
    {
        foreach (var observer in _observers)
        {
            await observer.UpdateAsync(command, parameters);
        }
    }
}