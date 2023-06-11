import Session, { SessionState } from "../Session/Session";
import Room from "./Room";

export default class RoomManager
{
    static Instance: RoomManager;

    // Debug
    static testRoom: Room | undefined;

    private roomMap: Map<string, Room>;

    constructor() {
        this.roomMap = new Map<string, Room>();
    }
    
    createRoom(name: string, owner: Session): Room | undefined {
        if(this.roomMap.has(name)) return;
        
        let room: Room = new Room(name, owner);
        this.roomMap.set(name, room);
        return room;
    }

    removeRoom(name: string): void {
        let room = this.roomMap.get(name);
        if(room) {
            room.members.forEach(session => {
                
            });
            this.roomMap.delete(name);
        }
    }
}