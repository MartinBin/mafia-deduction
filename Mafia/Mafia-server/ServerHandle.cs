namespace Mafia_server;

public class ServerHandle
{
    public delegate void Packet(int _playerID, byte[] _data);
    public static Dictionary<int, Packet> packets;
    
    private static void WelcomeReceived(int _playerID, byte[] _data)
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteBytes(_data);
        _buffer.ReadInt();
        string _username = _buffer.ReadString();
        _buffer.Dispose();
        Logger.Log(LogType.info2, "Connection from " + Globals.clients[_playerID].socket.Client.RemoteEndPoint + " was successful. Username: " + _username);
    }
    public static void InitPackets()
    {
        Logger.Log(LogType.info1, "Initializing packets..");
        packets = new Dictionary<int, Packet>()
        {
            { (int)ClientPackets.welcomeReceived, WelcomeReceived }
        };
    }
    public static void HandleData(int _playerID, byte[] _data)
    {
        byte[] _tempBuffer = (byte[])_data.Clone();
        int _packetLength = 0;
        
        if (Globals.clients[_playerID].buffer == null)
        {
            Globals.clients[_playerID].buffer = new ByteBuffer();
        }
        
        Globals.clients[_playerID].buffer.WriteBytes(_tempBuffer);
        if (Globals.clients[_playerID].buffer.Count() == 0)
        {
            Globals.clients[_playerID].buffer.Clear();
            return;
        }
        if (Globals.clients[_playerID].buffer.Length() >= 4)
        {
            _packetLength = Globals.clients[_playerID].buffer.ReadInt(false);
            if (_packetLength <= 0)
            {
                Globals.clients[_playerID].buffer.Clear();
                return;
            }
        }
        
        while (_packetLength > 0 && _packetLength <= Globals.clients[_playerID].buffer.Length() - 4)
        {
            Globals.clients[_playerID].buffer.ReadInt();
            _data = Globals.clients[_playerID].buffer.ReadBytes(_packetLength);
            HandlePackets(_playerID, _data);
            _packetLength = 0;
            if (Globals.clients[_playerID].buffer.Length() >= 4)
            {
                _packetLength = Globals.clients[_playerID].buffer.ReadInt(false);
                if (_packetLength <= 0)
                {
                    Globals.clients[_playerID].buffer.Clear();
                    return;
                }
            }
        }
        
        
        if (_packetLength <= 1)
        {
            Globals.clients[_playerID].buffer.Clear();
        }
    }
    
    private static void HandlePackets(int _playerID, byte[] _data)
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteBytes(_data);
        int _packetID = _buffer.ReadInt();
        _buffer.Dispose();
        
        if (packets.TryGetValue(_packetID, out Packet _packet))
        {
            _packet.Invoke(_playerID, _data);
        }
    }
}