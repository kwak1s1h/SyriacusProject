using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class RoomRemovedHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        RoomRemoved removeData = packet as RoomRemoved;
        if(removeData.IsYourRoom)
        {
            LobbyUI.Instance.SetCurrentWindow(LobbyUI.Instance.MultiModeContainer);
            LobbyUI.Instance.ClearRoomList();
            RoomListReq req = new RoomListReq();
            SocketManager.Instance.RegisterSend(MSGID.Roomlistreq, req);
            GameManager.Instance.PopupError("방이 삭제되었습니다.", "닫기");
        }
        else 
        {
            LobbyUI.Instance.DeleteRoomElement(removeData.RoomName);
        }
    }
}
