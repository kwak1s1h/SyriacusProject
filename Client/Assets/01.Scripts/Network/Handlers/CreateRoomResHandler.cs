using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class CreateRoomResHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        CreateRoomRes res = packet as CreateRoomRes;
        if(res.Success)
        {
            LobbyUI.Instance.InitRoom(res.Room.Name, res.Room.MaxCount, true);
            LobbyUI.Instance.SetCurrentWindow(LobbyUI.Instance.RoomContainer);  
        }
        else 
        {
            GameManager.Instance.PopupError("방 생성에 실패했습니다.", "닫기");
        }
        LobbyUI.Instance.CanInput = true;
    }
}
