using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class AttackHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        Attack atk = packet as Attack;

        if(RemoteManager.Instance.GetRemote(atk.Id, out RemoteInput remote))
        {
            remote.Attack();
        }
    }
}
