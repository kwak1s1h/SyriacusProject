import Session from "../Session/Session";
import { PacketHandler } from "../packet/PacketManager";
import { Attack, MSGID } from "../packet/packet";

const AttackHandler: PacketHandler = {
    code: MSGID.ATTACK,
    handle: function (session: Session, data: Uint8Array): void {
        if(session.room)
        {
            let attack = Attack.deserialize(data);
            attack.id = session.id;
            session.room.broadcast(attack.serialize(), MSGID.ATTACK, session);
        }
    }
}

module.exports = AttackHandler;