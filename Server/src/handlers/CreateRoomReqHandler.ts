import RoomManager from "../Room/RoomManager";
import Session from "../Session/Session";
import { PacketHandler } from "../packet/PacketManager";
import { CreateRoomReq, CreateRoomRes, MSGID, Room } from "../packet/packet";

const CreateRoomReqHandler: PacketHandler = {
    code: MSGID.CREATEROOMREQ,
    handle: function (session: Session, data: Uint8Array): void {
        let req = CreateRoomReq.deserialize(data);
        let room = RoomManager.Instance.createRoom(req.roomName, session, req.maxUser);

        let res: CreateRoomRes = new CreateRoomRes();
        if(room == undefined) {
            res.success = false;
        }
        else {
            res.success = true;
            res.room = new Room({
                name: room.name,
                maxCount: room.maxCnt,
                userCount: room.memberCnt
            });
        }
        session.sendData(res.serialize(), MSGID.CREATEROOMRES);
    }
};

module.exports = CreateRoomReqHandler;