using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Packet;
using UnityEngine;

public class PlayGameResHandler : IPacketHandler
{
    public void Process(IMessage packet)
    {
        PlayGameRes res = packet as PlayGameRes;
        if(res.Success)
        {
            GameManager.Instance.LoadSceneAsync("Game", true, (oper) => {
                GameManager.Instance.CreatePlayer(res.Type);
                foreach(Player p in res.Others)
                {
                    RemoteManager.Instance.CreateRemote(p.Type, p.Name, p.SpawnPos);
                }
            });
            LobbyUI.Instance.SetCurrentWindow(null);
        }
        else
            GameManager.Instance.PopupError("게임을 시작할 수 없습니다", "닫기");
    }
}
