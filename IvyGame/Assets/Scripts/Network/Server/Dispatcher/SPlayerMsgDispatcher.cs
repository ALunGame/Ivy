using Proto;
using UnityEngine;

namespace Game.Network.SDispatcher
{
    public class SPlayerMsgDispatcher : NetDispatcher
    {
        public SPlayerMsgDispatcher(NetDispatcherMapping InMapping) : base(InMapping)
        {
            //AddDispatch<JoinRoomC2s>(1,OnJoinRoomC2s);
            
            AddDispatch<JoinRoomS2c>((ushort)PlayerMsgDefine.JoinRoomS2c,OnJoinRoomS2c);

        }
        
        
        private void OnJoinRoomS2c(JoinRoomS2c MsgData)
        {

            Debug.Log(MsgData);
            //{

            //}
            //{ }
        
        }


    }
}

