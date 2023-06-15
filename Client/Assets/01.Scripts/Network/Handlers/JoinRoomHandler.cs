using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class JoinRoomHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        JoinRoom joinInfo = packet as JoinRoom;
        DevelopUI.Instance.SetUserCount(joinInfo.UserCount);
        Debug.Log(joinInfo.Id);
    }
}
