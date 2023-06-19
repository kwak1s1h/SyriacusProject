using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class RoomListResHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        RoomListRes res = packet as RoomListRes;
        LobbyUI.Instance.ClearRoomList();
        foreach(Room room in res.List)
        {
            LobbyUI.Instance.CreateRoomElement(room.Name, room.UserCount, room.MaxCount);
        }
    }
}
