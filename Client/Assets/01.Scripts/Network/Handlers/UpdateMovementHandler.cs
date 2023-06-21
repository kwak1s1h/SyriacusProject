using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class UpdateMovementHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        if(RemoteManager.Instance == null) return;
        UpdateMovement movement = packet as UpdateMovement;
        foreach(MoveData move in movement.List)
        {
            if(RemoteManager.Instance.GetRemote(move.Id, out RemoteInput remote))
            {
                remote.UpdateMovement(move);
            }
        }
    }
}
