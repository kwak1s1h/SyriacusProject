import Session, { PlayerType, SessionInfo, SessionState } from "../Session/Session";
import * as Packet from "../packet/packet";
import RoomManager from "./RoomManager";

export default class Room 
{
    name: string;
    owner: Session;
    members: Set<Session>;
    memberCnt: number;
    maxCnt: number;

    roomState: RoomState;

    updateInterval?: NodeJS.Timer;

    constructor(name: string, owner: Session, maxCnt: number) {
        this.name = name;
        this.owner = owner;
        this.maxCnt = maxCnt;
        this.members = new Set<Session>();
        this.members.add(owner);
        this.memberCnt = 1;
        this.roomState = RoomState.LOBBY;

        owner.room = this;
        owner.state = SessionState.INLOBBY;
        owner.playerType = PlayerType.TARGET;
    }

    join(session: Session): void {
        if(this.members.has(session)) return;
        this.members.add(session);
        session.room = this;
        this.memberCnt++;

        let joinInfo = new Packet.JoinRoom({id: session.id, userCount: this.memberCnt});
        this.broadcast(joinInfo.serialize(), Packet.MSGID.JOINROOM, session);
    }

    exit(session: Session): void {
        if(this.roomState == RoomState.LOBBY)
        {
            if(session.id == this.owner.id) {
                RoomManager.Instance.removeRoom(this.name);
                return;
            }
            this.members.delete(session);
            this.memberCnt--;
            session.room = undefined;
            session.state = SessionState.NONE;

            let exitData = new Packet.QuitRoom({
                id: session.id
            });
            this.broadcast(exitData.serialize(), Packet.MSGID.QUITROOM);
        }
        else 
        {

        }
    }

    play(): void {
        this.updateInterval = setInterval(() => {
            this.update();
        });
    }

    update(): void {

    }

    release(): void {
        clearInterval(this.updateInterval);
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
            users,
            maxCount: this.maxCnt,
            userCount: this.memberCnt
        };
        return info;
    }
}

export interface RoomInfo
{
    name: string;
    owner: SessionInfo;
    users: SessionInfo[];
    maxCount: number;
    userCount: number;
}

export enum RoomState
{
    LOBBY,
    INGAME
}