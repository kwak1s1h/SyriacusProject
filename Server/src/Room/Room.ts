import Session, { SessionInfo, SessionState } from "../Session/Session";
import { getRandomInt } from "../Utill";
import * as Packet from "../packet/packet";
import RoomManager from "./RoomManager";

export default class Room 
{
    name: string;
    owner: Session;
    members: Session[];
    memberCnt: number;
    maxCnt: number;

    roomState: RoomState;

    updateInterval?: NodeJS.Timer;

    constructor(name: string, owner: Session, maxCnt: number) {
        this.name = name;
        this.owner = owner;
        this.maxCnt = maxCnt;
        this.members = [];
        this.members.push(owner);
        this.memberCnt = 1;
        this.roomState = RoomState.LOBBY;

        owner.room = this;
        owner.state = SessionState.INLOBBY;
        owner.playerType = Packet.PlayerType.TARGET;
    }

    join(session: Session): void {
        if(this.members.find(s => s.id == session.id)) return;
        this.members.push(session);
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
            let idx = this.members.findIndex(s => s.id == session.id);
            this.members.splice(idx, 1);
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
        let confirm = new Packet.ConfirmLoad();
        this.broadcast(confirm.serialize(), Packet.MSGID.CONFIRMLOAD);

        let rand = getRandomInt(0, this.maxCnt);

        this.members.forEach((session, idx) => {
            session.playerType = idx == rand ? Packet.PlayerType.TARGET : Packet.PlayerType.CHASER;
        });

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

    checkAllReady(): boolean {
        let result = true;
        this.members.forEach(s => {
            if(s.state != SessionState.INGAME)
            {
                result = false;
                return;
            }
        });
        return result;
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