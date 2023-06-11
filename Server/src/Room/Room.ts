import Session from "../Session/Session";
import { MSGID } from "../packet/packet";
import RoomManager from "./RoomManager";

export default class Room 
{
    name: string;
    owner: Session;
    members: Set<Session>;

    constructor(name: string, owner: Session) {
        this.name = name;
        this.owner = owner;
        this.members = new Set<Session>();
        this.members.add(owner);

        owner.room = this;
    }

    join(session: Session): void {
        if(this.members.has(session)) return;

        this.members.add(session);
        session.room = this;
    }

    exit(session: Session): void {
        if(session.id == this.owner.id) {
            RoomManager.Instance.removeRoom(this.name);
            return;
        }

        this.members.delete(session);
        session.room = undefined;
    }

    broadcast(data: Uint8Array, code: number, sender?: Session): void {
        this.members.forEach((session) => {
            if(sender && session.id != sender.id) {
                return;
            }
            session.sendData(data, code);
        })

    }
}