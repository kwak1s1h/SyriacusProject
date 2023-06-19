import Session from "../Session/Session";
import { PacketHandler } from "../packet/PacketManager";
import { MSGID, PlayGameRes } from "../packet/packet";

const GamePlayReqHandler: PacketHandler = {
    code: MSGID.PLAYGAMEREQ,
    handle: function (session: Session, data: Uint8Array): void {
        let res = new PlayGameRes();
        if(session.room)
        {
            if(session.room.memberCnt <= 1)
            {
                res.
                session.sendData();
            }
            let play = new PlayGame();
            session.room.broadcast(play.serialize(), MSGID.PLAYGAME);
        }
    }
}

module.exports = GamePlayReqHandler;