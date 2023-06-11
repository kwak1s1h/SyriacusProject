import WS, { RawData } from "ws";
import { MSGID, Position } from "../packet/packet";
import PacketManager from "../packet/PacketManager";
import Room from "../Room/Room";

export default class Session
{
    socket: WS;
    state: SessionState
    id: string;

    // Room
    room: Room | undefined;

    // InGame Data
    position: Position;
    speed: number = 0;
    yRotation: number = 0;
    playerType: PlayerType;

    constructor(socket: WS, id: string) {
        this.socket = socket;
        // this.state = SessionState.DEFAULT;
        this.state = SessionState.INGAME;
        this.id = id;

        this.position = new Position({x: 0, y: 0, z: 0});
        this.speed = 0;
        this.playerType = PlayerType.NONE;
    }

    sendData(data: Uint8Array, code: MSGID): void {
        let len: number = data.length + 4;

        let lenBuffer: Uint8Array = new Uint8Array(2); 
        new DataView(lenBuffer.buffer).setUint16(0, len, true);

        let msgCodeBuffer: Uint8Array = new Uint8Array(2);
        new DataView(msgCodeBuffer.buffer).setUint16(0, code, true);

        let sendBuffer: Uint8Array = new Uint8Array(len);
        sendBuffer.set(lenBuffer, 0);
        sendBuffer.set(msgCodeBuffer, 2);
        sendBuffer.set(data, 4);

        this.socket.send(sendBuffer);
    }

    private getInt16LEFromBuffer(buffer:Buffer): number {
        return buffer.readInt16LE();
    }

    receiveMessage(data: RawData): void {
        let code:number = this.getInt16LEFromBuffer(data.slice(2, 4) as Buffer);
        PacketManager.Instance.handleMessage(this, data.slice(4) as Buffer, code);
    }
}

export enum SessionState
{
    DEFAULT,
    INGAME
}

export enum PlayerType
{
    NONE,
    CHASER,
    TARGET
}