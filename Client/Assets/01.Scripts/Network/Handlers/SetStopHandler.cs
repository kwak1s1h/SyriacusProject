using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class SetStopHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        SetStop stop = packet as SetStop;

        GameManager.Instance.Player.CanMove = !stop.Value;
        GameUI.Instance.SetStop(stop.Value);
    }
}
