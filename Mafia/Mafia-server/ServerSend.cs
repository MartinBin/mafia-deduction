namespace Mafia_server;

public class ServerSend
{
    public static void SendDataTo(int _playerID, byte[] _data)
    {
        try
        {
            if (Globals.clients[_playerID].socket != null)
            {
                ByteBuffer _buffer = new ByteBuffer();
                _buffer.WriteInt(_data.GetUpperBound(0) - _data.GetLowerBound(0) + 1);
                _buffer.WriteBytes(_data);
                
                Globals.clients[_playerID].stream.BeginWrite(_buffer.ToArray(), 0, _buffer.ToArray().Length, null, null);
                _buffer.Dispose();
            }
        }
        catch (Exception _ex)
        {
            Logger.Log(LogType.error, "Error sending data to player " + _playerID + ": " + _ex);
        }
    }
    
    public static void Welcome(int _sendToPlayer, string _msg)
    {
        ByteBuffer _buffer = new ByteBuffer();
        _buffer.WriteInt((int)ServerPackets.welcome);
        
        _buffer.WriteString(_msg);
        _buffer.WriteInt(_sendToPlayer);
        
        SendDataTo(_sendToPlayer, _buffer.ToArray());
        _buffer.Dispose();
    }
}