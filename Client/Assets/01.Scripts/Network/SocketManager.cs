using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.WebSockets;
using Google.Protobuf;
using System.Threading.Tasks;

public class SocketManager : MonoBehaviour
{
    public static SocketManager Instance;

    private ClientWebSocket _socket = null;
    private string _url;
    
    [SerializeField] private bool _isConnected = false;
    private bool _isReadyToSend = false
    ;
    private RecvBuffer _recvBuffer;
    private PacketManager _packetManager;
    private Queue<PacketMessage> _sendQueue;

    public event Action OnConnect;
    public event Action OnDisconnect;

    public void Init(string url)
    {
        _url = url;
        _recvBuffer = new RecvBuffer(1024 * 10);
        _packetManager = new PacketManager();
        _sendQueue = new Queue<PacketMessage>();
        _isConnected = false;
    }

    public async Task Connection()
    {
        if(_socket != null && _socket.State == WebSocketState.Open)
        {
            return;
        }

        _socket = new ClientWebSocket();
        Uri serverUri = new Uri(_url);
        try
        {
            await _socket.ConnectAsync(serverUri, CancellationToken.None);
            _isConnected = true;
            OnConnect?.Invoke();
            ReceiveLoop();
        }
        catch(Exception ex)
        {
            Debug.LogError("Connection Error : check server status... " + ex.Message);
            _isConnected = false;
            OnDisconnect?.Invoke();
            throw;
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    private void Update()
    {
        if (PacketQueue.Instance.Count > 0)
        {
            List<PacketMessage> list = PacketQueue.Instance.PopAll();
            foreach (PacketMessage pmsg in list)
            {
                IPacketHandler handler = _packetManager.GetPacketHandler(pmsg.Id);
                if (handler != null)
                {
                    handler.Process(pmsg.Message);
                }
                else
                {
                    Debug.LogError($"There is no handler for this packet : {pmsg.Id}");
                }
            }
        }

        if (_isReadyToSend && _sendQueue.Count > 0)
        {
            SendMessages();
        }
    }

    public void RegisterSend(ushort code, IMessage msg)
    {
        _sendQueue.Enqueue(new PacketMessage { Id = (ushort)code, Message = msg });
    }

    private async void SendMessages()
    {
        if (_socket != null && _socket.State == WebSocketState.Open)
        {
            _isReadyToSend = false; 

            List<PacketMessage> sendList = new List<PacketMessage>();
            while (_sendQueue.Count > 0)
            {
                sendList.Add(_sendQueue.Dequeue());
            }

            byte[] sendBuffer = new byte[1024 * 10];
            foreach (PacketMessage pmsg in sendList)
            {
                int len = pmsg.Message.CalculateSize(); 

                Array.Copy(BitConverter.GetBytes((ushort)(len + 4)), 0, sendBuffer, 0, sizeof(ushort));
                Array.Copy(BitConverter.GetBytes(pmsg.Id), 0, sendBuffer, 2, sizeof(ushort));
                Array.Copy(pmsg.Message.ToByteArray(), 0, sendBuffer, 4, len);

                ArraySegment<byte> segment = new ArraySegment<byte>(sendBuffer, 0, len + 4);
                await _socket.SendAsync(segment, WebSocketMessageType.Binary, true, CancellationToken.None);
            }

            _isReadyToSend = true; 
        }
    }

    private async void ReceiveLoop()
    {
        while (_socket != null && _socket.State == WebSocketState.Open)
        {
            try
            {
                WebSocketReceiveResult result
                    = await _socket.ReceiveAsync(_recvBuffer.WriteSegment, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    if (result.EndOfMessage == true)
                    {
                        _recvBuffer.OnWrite(result.Count);
                        int readByte = ProcessPacket(_recvBuffer.ReadSegment);
                        if (readByte == 0)
                        {
                            Disconnect();
                            break;
                        }

                        _recvBuffer.OnRead(readByte);
                        _recvBuffer.Clean();
                    }
                    else
                    {
                        _recvBuffer.OnWrite(result.Count);
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Debug.LogError("Socket closed by server in normal status");
                    OnDisconnect?.Invoke();
                    break;
                }
            }
            catch (WebSocketException we)
            {
                Debug.LogError(we.Message);
                OnDisconnect?.Invoke();
                break;
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.GetType()} : {e.Message}");
                OnDisconnect?.Invoke();
                break;
            }
        }
    }

    private int ProcessPacket(ArraySegment<byte> buffer)
    {
        return _packetManager.OnRecvPacket(buffer);
    }

    public void Disconnect()
    {
        if (_socket != null && _socket.State == WebSocketState.Open)
        {
            _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "quit_client", CancellationToken.None);
            _isConnected = false;
            OnDisconnect?.Invoke();
        }
    }
}
