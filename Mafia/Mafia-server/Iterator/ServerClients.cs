namespace Mafia_server.Iterator
{
    public class ServerClients : IAggregate<Client>
    {
        private List<Client> clients = new List<Client>();

        public void AddClient(Client client)
        {
            clients.Add(client);
        }

        public void RemoveClient(Client client)
        {
            clients.Remove(client);
        }

        public IIterator<Client> CreateIterator()
        {
            return new ClientIterator(this);
        }

        public List<Client> GetClients()
        {
            return clients;
        }
    }
}
