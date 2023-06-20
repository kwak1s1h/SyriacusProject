import Session from "../Session/Session";
import { PacketHandler } from "../packet/PacketManager";
import { MSGID, PlayGameRes } from "../packet/packet";

const GamePlayReqHandler: PacketHandler = {
    code: MSGID.PLAYGAMEREQ,
    handle: function (session: Session, data: Uint8Array): void {
        let res = new PlayGameRes({success: true});
        if(session.room)
        {
            if(session.room.memberCnt <= 1)
            {
                res.success = false;
                session.sendData(res.serialize(), MSGID.PLAYGAMERES);
            }
            else
                session.room.broadcast(res.serialize(), MSGID.PLAYGAMERES);
        }
    }
}

module.exports = GamePlayReqHandler;