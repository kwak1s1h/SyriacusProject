import Session from "../Session/Session";
import { PacketHandler } from "../packet/PacketManager";
import { MSGID, MsgBox } from "../packet/packet";

export const MsgBoxHandler: PacketHandler = {
    code: MSGID.MSGBOX,
    handle: function (session: Session, data: Uint8Array): void {
        let box = MsgBox.deserialize(data);
        console.log(`${session.id}: ${box.context}`);
    }
}