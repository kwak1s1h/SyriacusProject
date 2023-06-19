import RoomManager from "../Room/RoomManager";
import Session from "../Session/Session";
import { PacketHandler } from "../packet/PacketManager";
import * as Packet from "../packet/packet";

const RoomListReqHandler: PacketHandler = {
    code: Packet.MSGID.ROOMLISTREQ,
    handle: function (session: Session, data: Uint8Array): void {
        let res = new Packet.RoomListRes();
        RoomManager.Instance.getAllRoomInfo().forEach(info => {
            res.list.push(new Packet.Room({
                maxCount: info.maxCount,
                name: info.name,
                userCount: info.users.length
            }));
        });
        session.sendData(res.serialize(), Packet.MSGID.ROOMLISTRES);
    }
}

module.exports = RoomListReqHandler;