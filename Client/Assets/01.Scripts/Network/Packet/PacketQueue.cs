using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketMessage
{
    public ushort Id;
    public IMessage Message;
}

public class PacketQueue
{
    public static PacketQueue Instance = new PacketQueue();

    private Queue<PacketMessage> _packetQueue = new Queue<PacketMessage>();

    public int Count => _packetQueue.Count;

    object _lock = new object(); //쓰레드 락을 위한 빈 옵젝 하나

    public void Push(ushort id, IMessage packet)
    {
        lock (_lock)
        {
            _packetQueue.Enqueue(new PacketMessage { Id = id, Message = packet });
        }
    }

    public PacketMessage Pop()
    {
        lock (_lock)
        {
            if (_packetQueue.Count == 0) return null;

            return _packetQueue.Dequeue();
        }
    }

    public List<PacketMessage> PopAll()
    {
        List<PacketMessage> list = new List<PacketMessage>();

        lock (_lock)
        {
            while (_packetQueue.Count > 0)
                list.Add(_packetQueue.Dequeue());
        }

        return list;
    }
}
