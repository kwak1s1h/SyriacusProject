import Room from "../Room/Room";
import RoomManager from "../Room/RoomManager";
import Session, { SessionInfo } from "./Session";

export default class SessionManager
{
    static Instance: SessionManager;

    private sessionMap: { [key: string]: Session | undefined };
    sessionCnt: number;

    constructor() {
        this.sessionMap = {};
        this.sessionCnt = 0;
    }

    addSession(session: Session) {
        if(!this.sessionMap[session.id])
        this.sessionMap[session.id] = session;

        // Debug
        console.log(this.sessionCnt);
        if(this.sessionCnt <= 0) {
            RoomManager.testRoom = RoomManager.Instance.createRoom("test", session);
        }
        else RoomManager.testRoom?.join(session);

        this.sessionCnt++;
    }

    removeSession(id: string): void {
        let session = this.sessionMap[id];
        if(session) {
            if(session.room) {
                session.room.exit(session);
            }
            delete this.sessionMap[id];
            this.sessionCnt--;
        }
    }

    getSession(id: string): Session | undefined {
        return this.sessionMap[id];
    }

    broadcastAll(data: Uint8Array, code: number): void {
        Object.values(this.sessionMap).forEach((session) => {
            if(session)
                session.sendData(data, code);
        });
    }

    getAllSessionInfo(): SessionInfo[] {
        let list: SessionInfo[] = [];
        Object.values(this.sessionMap).forEach(session => {
            if(session)
                list.push(session.getSessionInfo());
        });
        return list;
    }
}