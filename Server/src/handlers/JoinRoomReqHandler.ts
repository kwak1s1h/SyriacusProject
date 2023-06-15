import RoomManager from "../Room/RoomManager";
import Session from "../Session/Session";
import { PacketHandler } from "../packet/PacketManager";
import { JoinRoomReq, JoinRoomRes, MSGID } from "../packet/packet";

module.exports = {
    code: MSGID.JOINROOMREQ,
    handle: function (session: Session, data: Uint8Array): void {
        let req = JoinRoomReq.deserialize(data);
        let res = new JoinRoomRes();
        let room = RoomManager.Instance.getRoom(req.roomName);
        if(room && !session.room) 
        {
            room.join(session);
            res.success = true;
            res.userCount = room.members.size;
        }
        else
        {
            res.success = false;
        }
        session.sendData(res.serialize(), MSGID.JOINROOMRES);
    }
}