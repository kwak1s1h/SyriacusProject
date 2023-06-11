import fs from "fs/promises";
import path from "path";
import Session from "../Session/Session";
import { MSGID } from "./packet";

export default class PacketManager
{
    static Instance: PacketManager

    private handlerMap: Map<MSGID, PacketHandler>;

    constructor() {
        this.handlerMap = new Map<MSGID, PacketHandler>();
        this.registerHandlers();
    }

    async registerHandlers() {
        let target = path.join(__dirname, "..", "handlers");
        let handlerDir = await fs.readdir(target);
        handlerDir.forEach(async (fileName, idx) => {
            let handler: PacketHandler = await import(path.join(target, fileName));
            this.handlerMap.set(handler.code, handler);
        });
    }

    handleMessage(session: Session, data: Uint8Array, code: MSGID): void {
        let handler = this.handlerMap.get(code);
        if(handler) handler.handle(session, data);
        else throw new Error("Handler Not Found.");
    }
}

export interface PacketHandler
{
    code: number;
    handle(session:Session, data: Uint8Array): void;
}