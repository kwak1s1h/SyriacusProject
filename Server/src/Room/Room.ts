import Session, { PlayerType, SessionInfo, SessionState } from "../Session/Session";
import { JoinRoom, JoinRoomRes, MSGID } from "../packet/packet";
import RoomManager from "./RoomManager";

export default class Room 
{
    name: string;
    owner: Session;
    members: Set<Session>;
    memberCnt: number;

    constructor(name: string, owner: Session) {
        this.name = name;
        this.owner = owner;
        this.members = new Set<Session>();
        this.members.add(owner);
        this.memberCnt = 1;

        owner.room = this;
        owner.state = SessionState.INLOBBY;
        owner.playerType = PlayerType.TARGET;
    }

    join(session: Session): void {
        if(this.members.has(session)) return;
        this.members.add(session);
        session.room = this;
        this.memberCnt++;

        let joinInfo = new JoinRoom({id: session.id, userCount: this.memberCnt});
        this.broadcast(joinInfo.serialize(), MSGID.JOINROOM, session);

        let joinRes = new JoinRoomRes({success: true, userCount: this.memberCnt});
        session.sendData(joinRes.serialize(), MSGID.JOINROOMRES);
    }

    exit(session: Session): void {
        if(session.id == this.owner.id) {
            RoomManager.Instance.removeRoom(this.name);
            // TODO: Send room has removed to all user
            return;
        }
        this.members.delete(session);
        this.memberCnt--;
        session.room = undefined;
    }

    play(): void {
        
    }

    broadcast(data: Uint8Array, code: number, sender?: Session): void {
        console.log(`sender: ${sender?.id}`);
        this.members.forEach((session) => {
            console.log(`compare: ${session.id}`);
            if(sender != undefined && session.id == sender.id) {
                console.log("expect");
                return;
            }
            else session.sendData(data, code);
        });
    }

    getInfo(): RoomInfo {
        let users: SessionInfo[] = [];
        this.members.forEach(session => {
            users.push(session.getSessionInfo());
        })
        let info: RoomInfo = {
            name: this.name,
            owner: this.owner.getSessionInfo(),
            users
        };
        return info;
    }
}

export interface RoomInfo
{
    name: string;
    owner: SessionInfo
    users: SessionInfo[]
}