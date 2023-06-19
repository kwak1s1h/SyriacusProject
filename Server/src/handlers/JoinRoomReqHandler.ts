import RoomManager from "../Room/RoomManager";
import Session from "../Session/Session";
import { PacketHandler } from "../packet/PacketManager";
import * as Packet from "../packet/packet";

module.exports = {
    code: Packet.MSGID.JOINROOMREQ,
    handle: function (session: Session, data: Uint8Array): void {
        let req = Packet.JoinRoomReq.deserialize(data);
        let res = new Packet.JoinRoomRes();
        let room = RoomManager.Instance.getRoom(req.roomName);
        console.log(room?.name, room?.maxCnt, room?.memberCnt);
        if(room && !session.room && room.maxCnt > room.memberCnt) 
        {
            room.join(session);
            res.success = true;
            room.members.forEach(s => {
                if(s.id != session.id)
                    res.userList.push(s.id);
            });
            res.room = new Packet.Room({
                maxCount: room.maxCnt,
                name: room.name,
                userCount: room.memberCnt
            });
        }
        else
        {
            res.success = false;
        }
        session.sendData(res.serialize(), Packet.MSGID.JOINROOMRES);
    }
}