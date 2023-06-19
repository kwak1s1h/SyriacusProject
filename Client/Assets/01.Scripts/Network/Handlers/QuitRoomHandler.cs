using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class QuitRoomHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        QuitRoom quit = packet as QuitRoom;
        LobbyUI.Instance.DeleteUserElement(quit.Id);
    }
}
