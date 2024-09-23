using System.Net.Sockets;

namespace Mafia_server;

public class Client
{
    public int playerID;
    public bool isPlaying = false;

    public TcpClient socket;
    public NetworkStream stream;

    public ByteBuffer buffer;
    public Player player;

    private byte[] receiveBuffer;

    public void StartClient()
    {
        socket.ReceiveBufferSize = 4096;
        socket.SendBufferSize = 4096;

        stream = socket.GetStream();
        receiveBuffer = new byte[socket.ReceiveBufferSize];
        stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, ReceivedData, null);
        player = new Player(playerID);
        ServerSend.Welcome(playerID, "Welcome to the server!");
    }
    
    private void ReceivedData(IAsyncResult _result)
    {
        try
        {
            int _byteLength = stream.EndRead(_result);
            if (_byteLength <= 0)
            {
                CloseConnection();
                return;
            }
            byte[] _tempBuffer = new byte[_byteLength];
            Array.Copy(receiveBuffer, _tempBuffer, _byteLength);
            ServerHandle.HandleData(playerID, _tempBuffer);
            stream.BeginRead(receiveBuffer, 0, socket.ReceiveBufferSize, ReceivedData, null);
        }
        catch (Exception _ex)
        {
            Logger.Log(LogType.error, "Error while receiving data: " + _ex);
            return;
        }
    }
    
    private void CloseConnection()
    {
        Logger.Log(LogType.info1, "Connection from " + socket.Client.RemoteEndPoint.ToString() + " has been terminated");
        player = null;
        isPlaying = false;
        socket.Close();
        socket = null;
    }

}