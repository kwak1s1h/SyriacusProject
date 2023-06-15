import { ExtensionFieldInfo } from "google-protobuf";
import Session, { SessionState } from "../Session/Session";
import { PacketHandler } from "../packet/PacketManager";
import { MSGID, MoveData } from "../packet/packet";

module.exports = {
    code: MSGID.MOVEDATA,
    handle: function (session: Session, data: Uint8Array): void {
        if(session.state != SessionState.INGAME || !session.room) return;

        let moveData: MoveData = MoveData.deserialize(data);
        session.speed = moveData.speed;
        session.yRotation = moveData.yRotation;
        session.position = moveData.pos;
    }
}