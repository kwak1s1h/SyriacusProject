using System.Linq;
using System.IO;
using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Packet;

public class PacketManager
{
    private Dictionary<MSGID, Action<ArraySegment<byte>, ushort>> _OnRecv;
    private Dictionary<MSGID, IPacketHandler> _Handlers;

    public PacketManager()
    {
        _OnRecv = new Dictionary<MSGID, Action<ArraySegment<byte>, ushort>>();
        _Handlers = new Dictionary<MSGID, IPacketHandler>();
        Register();
    }

    private void Register()
    {
        _OnRecv.Add(MSGID.Msgbox, MakePacket<MsgBox>);
        _Handlers.Add(MSGID.Msgbox, new MsgBoxHandler());
        
        _OnRecv.Add(MSGID.Joinroomres, MakePacket<JoinRoomRes>);
        _Handlers.Add(MSGID.Joinroomres, new JoinRoomResHandler());

        _OnRecv.Add(MSGID.Roomcreated, MakePacket<Room>);
        _Handlers.Add(MSGID.Roomcreated, new RoomCreatedHandler());

        _OnRecv.Add(MSGID.Joinroom, MakePacket<JoinRoom>);
        _Handlers.Add(MSGID.Joinroom, new JoinRoomHandler());
    }

    public IPacketHandler GetPacketHandler(ushort id)
    {
        IPacketHandler hanlder = null;
        if (_Handlers.TryGetValue((MSGID)id, out hanlder))
        {
            return hanlder;
        }
        else
        {
            return null;
        }
    }

    public int OnRecvPacket(ArraySegment<byte> buffer)
    {
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset); //2바이트 긁는다.
        ushort code = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2); // 뒤에 2바이트 긁는다.

        if (_OnRecv.ContainsKey((MSGID)code))
        {
            _OnRecv[(MSGID)code].Invoke(buffer, code);
        }
        else
        {
            Debug.LogError($"There is no packet handler for this packet : {((MSGID)code).ToString()}, ({size}");
            return 0;
        }
        return size;
    }

    private void MakePacket<T>(ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
    {
        T pkt = new T();
        pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

        PacketQueue.Instance.Push(id, pkt);
    }
}
