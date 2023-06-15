using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class RoomCreatedHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        Room room = packet as Room;
    }
}
