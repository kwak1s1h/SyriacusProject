import Room from "../Room/Room";
import RoomManager from "../Room/RoomManager";
import Session from "./Session";

export default class SessionManager
{
    static Instance: SessionManager;

    private sessionMap: Map<string, Session>;

    constructor() {
        this.sessionMap = new Map<string, Session>();
    }

    addSession(session: Session) {
        // Debug
        if(this.sessionMap.size <= 0) {
            RoomManager.testRoom = RoomManager.Instance.createRoom("test", session);
        }

        if(!this.sessionMap.has(session.id))
            this.sessionMap.set(session.id, session);
    }

    removeSession(id: string): void {
        let session = this.sessionMap.get(id);
        if(session) {
            if(session.room) {
                session.room.exit(session);
            }
            this.sessionMap.delete(id);
        }
    }

    getSession(id: string): Session | undefined {
        return this.sessionMap.get(id);
    }

    broadcastAll(data: Uint8Array, code: number): void {
        this.sessionMap.forEach((session, id) => {
            session.sendData(data, code);
        });
    }
}