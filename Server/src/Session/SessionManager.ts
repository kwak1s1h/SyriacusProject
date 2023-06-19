import Room from "../Room/Room";
import RoomManager from "../Room/RoomManager";
import Session, { SessionInfo } from "./Session";

export default class SessionManager
{
    static Instance: SessionManager;

    private sessionMap: Map<string, Session>;
    sessionCnt: number;

    constructor() {
        this.sessionMap = new Map<string, Session>();
        this.sessionCnt = 0;
    }

    addSession(session: Session) {
        if(!this.sessionMap.has(session.id))
        {
            this.sessionMap.set(session.id, session);
            this.sessionCnt++;
        }
    }

    removeSession(id: string): void {
        let session = this.getSession(id);
        if(session) {
            if(session.room) {
                session.room.exit(session);
            }
            this.sessionMap.delete(id);
            this.sessionCnt--;
        }
    }

    getSession(id: string): Session | undefined {
        return this.sessionMap.get(id);
    }

    broadcastAll(data: Uint8Array, code: number): void {
        this.sessionMap.forEach((session) => {
            if(session)
                session.sendData(data, code);
        });
    }

    broadcastByPredicate(data: Uint8Array, code: number, predicate: (sesison: Session) => boolean)
    {
        this.sessionMap.forEach((session) => {
            if(session && predicate(session))
                    session.sendData(data, code);
        });
    }

    getAllSessionInfo(): SessionInfo[] {
        let list: SessionInfo[] = [];
        this.sessionMap.forEach(session => {
            if(session)
                list.push(session.getSessionInfo());
        });
        return list;
    }
}