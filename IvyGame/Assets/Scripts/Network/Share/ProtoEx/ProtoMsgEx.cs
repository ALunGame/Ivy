using Proto;

namespace Game.Network
{
    public static class ProtoMsgEx
    {
        public static GamerInputC2s Clone(this GamerInputC2s pMsg)
        {
            GamerInputC2s newMsg = new GamerInputC2s();
            newMsg.gamerUid = pMsg.gamerUid;
            newMsg.commandType = pMsg.commandType;
            newMsg.Rotation = pMsg.Rotation;
            newMsg.serverTick = pMsg.serverTick;
            newMsg.commandTick = pMsg.commandTick;
            return newMsg;
        }
    }
}
