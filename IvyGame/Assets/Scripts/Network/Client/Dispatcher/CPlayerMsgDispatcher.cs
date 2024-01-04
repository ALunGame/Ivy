using Proto;
using UnityEngine;

namespace Game.Network.CDispatcher
{
    public class CPlayerMsgDispatcher : NetDispatcher
    {
        public CPlayerMsgDispatcher(NetDispatcherMapping InMapping) : base(InMapping)
        {
            //AddDispatch<JoinRoomC2s>(1,OnJoinRoomC2s);
            
            AddDispatch<JoinRoomC2s>((int)PlayerMsgDefine.JoinRoomC2s,OnJoinRoomC2s);

        }
        
        
        private void OnJoinRoomC2s(JoinRoomC2s MsgData)
        {

            Debug.Log(MsgData);
            //{

            //}
            //{ }  
        
        }


    }
}

