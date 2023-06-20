import Session, { SessionState } from "../Session/Session";
import SessionManager from "../Session/SessionManager";
import { PacketHandler } from "../packet/PacketManager";
import { MSGID } from "../packet/packet";

const SceneReadyHandler: PacketHandler = {
    code: MSGID.SCENEREADY,
    handle: function (session: Session, data: Uint8Array): void {
        session.state = SessionState.INGAME;
        if(session.room?.checkAllReady())
            session.room?.play();
    }
}

module.exports = SceneReadyHandler;