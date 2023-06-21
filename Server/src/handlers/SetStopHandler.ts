import Session, { SessionState } from "../Session/Session";
import { PacketHandler } from "../packet/PacketManager";
import { MSGID, SetStop } from "../packet/packet";

const SetStopHandler: PacketHandler = {
    code: MSGID.SETSTOP,
    handle: function (session: Session, data: Uint8Array): void {
        let stop = SetStop.deserialize(data);
        if(session.room && session.state == SessionState.INGAME)
        {
            let target = session.room.members.find(s => s.id == stop.id);

            if(target?.state == SessionState.INGAME)
                target.sendData(data, MSGID.SETSTOP);
        }
    }
}

module.exports = SetStopHandler;