using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using UnityEngine;

public class ConfirmLoadHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        SocketManager.Instance.LoadSceneAllow = true;
    }
}
