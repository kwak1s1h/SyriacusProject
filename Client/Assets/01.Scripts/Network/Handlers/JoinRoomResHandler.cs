using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class JoinRoomResHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        JoinRoomRes res = packet as JoinRoomRes;
        if(res.Success)
        {
            LobbyUI.Instance.InitRoom(res.Room.Name, res.Room.MaxCount);
            foreach(string name in res.UserList)
            {
                LobbyUI.Instance.CreateUserElement(name);
            }
            LobbyUI.Instance.SetCurrentWindow(LobbyUI.Instance.RoomContainer);
        }
        else 
        {
            GameManager.Instance.PopupError("참가에 실패했습니다.", "닫기");
        }
        LobbyUI.Instance.CanInput = true;
    }
}
