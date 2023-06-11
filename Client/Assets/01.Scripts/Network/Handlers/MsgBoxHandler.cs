using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class MsgBoxHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        MsgBox box = packet as MsgBox;
        Debug.Log(box.Context);
    }
}
