using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class JoinRoomResHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        JoinRoomRes res = packet as JoinRoomRes;
        Debug.Log("Success: " + res.Success);
        if(res.Success) {
            DevelopUI.Instance.SetUserCount(res.UserCount);
        }
    }
}
