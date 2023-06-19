import Session, { SessionState } from "../Session/Session";
import SessionManager from "../Session/SessionManager";
import Room, { RoomInfo } from "./Room";
import * as Packet from "../packet/packet";

export default class RoomManager
{
    static Instance: RoomManager;

    private roomMap: Map<string, Room>;

    constructor() {
        this.roomMap = new Map<string, Room>();
    }
    
    createRoom(name: string, owner: Session, maxCnt: number): Room | undefined {
        if(this.roomMap.has(name)) return;
        if(maxCnt < 2 || maxCnt > 6) return;
        if(name.trim() == "") return;
        
        let room: Room = new Room(name, owner, maxCnt);
        this.roomMap.set(name, room);

        let createData: Packet.Room = new Packet.Room({
            maxCount: room.maxCnt,
            name: room.name,
            userCount: room.memberCnt
        });
        SessionManager.Instance.broadcastByPredicate(
            createData.serialize(), Packet.MSGID.ROOMCREATED,
            (session) => {
                return session.id != owner.id && session.state == SessionState.NONE;
            }
        );
        return room;
    }

    removeRoom(name: string): void {
        let room = this.roomMap.get(name);
        if(room) {
            let removeData = new Packet.RoomRemoved({
                roomName: room.name,
                isYourRoom: false
            });
            SessionManager.Instance.broadcastByPredicate(
                removeData.serialize(), Packet.MSGID.ROOMREMOVED,
                (s) => s.room?.name != room?.name
            );
            room.members.forEach(session => {
                // Send Exit Data
                removeData.isYourRoom = true;
                session.sendData(removeData.serialize(), Packet.MSGID.ROOMREMOVED);
                session.state = SessionState.NONE;
                delete session.room;
            });
            this.roomMap.delete(name);
        }
    }

    getRoom(name: string): Room | undefined {
        return this.roomMap.get(name);
    }

    getAllRoomInfo(): RoomInfo[] {
        let result: RoomInfo[] = [];
        this.roomMap.forEach(room => {
            result.push(room.getInfo());
        });
        return result;
    }
}