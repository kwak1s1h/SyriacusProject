using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class PlayGameResHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        // PlayGame play = packet as PlayGame;
        GameManager.Instance.LoadSceneAsync("Game", true);
    }
}
