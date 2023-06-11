import Express from "express";
import WS from "ws";
import Session from "./Session/Session";
import crypto from "crypto";
import PacketManager from "./packet/PacketManager";
import SessionManager from "./Session/SessionManager";
import RoomManager from "./Room/RoomManager";
import Room from "./Room/Room";

const App = Express();

const httpServer = App.listen(30000);

SessionManager.Instance = new SessionManager();
RoomManager.Instance = new RoomManager();
PacketManager.Instance = new PacketManager();

const wsServer = new WS.Server({ server: httpServer });

wsServer.on("listening", () => {
    console.log("서버 켜짐");
});

wsServer.on("connection", (socket, req) => {
    console.log("새로운 클라 들어옴");
    let session = new Session(socket, crypto.randomUUID());
    
    socket.on("message", (data: WS.RawData, isBinary: boolean) => {
        if(isBinary)
            session.receiveMessage(data);
    });

    socket.on('close', (code, reason) => {
        if(session.room) session.room.exit(session);
        console.log(`클라 나감 이유는 ${reason.toString()}`);
    });
});