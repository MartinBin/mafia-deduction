namespace Mafia_server.Iterator
{
    public class ClientIterator : IIterator<Client>
    {
        private readonly ServerClients _serverClients;
        private int _position = -1;

        public ClientIterator(ServerClients clients)
        {
            _serverClients = clients;
        }

        public bool HasNext()
        {
            return (_position < _serverClients.GetClients().Count - 1);
        }

        public void MoveNext()
        {
            _position++;
        }

        public Client CurrentItem()
        {
            if (_position < 0 || _position >= _serverClients.GetClients().Count)
            {
                throw new InvalidOperationException();
            }
            return _serverClients.GetClients()[_position];
        }
    }

}
