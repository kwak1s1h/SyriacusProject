using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;
//#include <iostream>

public class SetPlayerHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        SetPlayer player = packet as SetPlayer;
        GameManager.Instance.CreatePlayer(player.Type);
    }

    // cout << "재희 빡빡";
}
