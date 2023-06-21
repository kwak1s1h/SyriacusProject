import Session from "../Session/Session";
import { getRandomInt } from "../Utill";
import { PacketHandler } from "../packet/PacketManager";
import { MSGID, PlayGameRes, Player, PlayerType, Position } from "../packet/packet";

const GamePlayReqHandler: PacketHandler = {
    code: MSGID.PLAYGAMEREQ,
    handle: function (session: Session, data: Uint8Array): void {
        let res = new PlayGameRes({success: true});
        if(session.room)
        {
            let room = session.room;
            if(room.memberCnt <= 1)
            {
                res.success = false;
                session.sendData(res.serialize(), MSGID.PLAYGAMERES);
            }
            else
            {
                let rand = getRandomInt(0, room.maxCnt);
                room.members.forEach((s, idx) => {
                    s.playerType = rand == idx ? PlayerType.TARGET : PlayerType.CHASER;
                    res.type = s.playerType;
                });
                room.members.forEach(s => {
                    res.type = s.playerType;
                    res.others = room.members.filter(se => se.id != s.id).map(se => new Player({
                        name: se.id, spawnPos: new Position({x: 0, y: 1, z: 0}), type: se.playerType
                    }));
                    s.sendData(res.serialize(), MSGID.PLAYGAMERES);
                });
            }
        }
    }
}

module.exports = GamePlayReqHandler;