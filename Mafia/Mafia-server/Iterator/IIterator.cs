namespace Mafia_server.Iterator
{
    public interface IIterator<T>
    {
        bool HasNext();
        void MoveNext();
        T CurrentItem();
    }
}
