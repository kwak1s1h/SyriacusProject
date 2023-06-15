import Session from "../Session/Session";
import { PacketHandler } from "../packet/PacketManager";
import { MSGID } from "../packet/packet";

module.exports = {
    code: MSGID.QUITROOM,
    handle: function (session: Session, data: Uint8Array): void {
        let room = session.room;
        if(room) 
        {
            room.exit(session);
        }
    }
}