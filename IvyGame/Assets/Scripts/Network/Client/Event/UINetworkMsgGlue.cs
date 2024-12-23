using IAUI;
using ProtoBuf;
using System;

namespace Game.Network.Client
{
    public class UINetworkMsgGlue : UIGlue
    {
        private ushort msgId;
        private Action<IExtensible> msgFunc;

        public UINetworkMsgGlue(ushort pMsgId, Action<IExtensible> pMsgFunc)
        {
            msgId = pMsgId;
            msgFunc = pMsgFunc;
        }

        public UINetworkMsgGlue(InternalUIPanel pPanel, ushort pMsgId, Action<IExtensible> pMsgFunc) : base(pPanel)
        {
            msgId = pMsgId;
            msgFunc = pMsgFunc;
        }

        public override void OnAfterShow(InternalUIPanel panel)
        {
            base.OnAfterShow(panel);
            NetworkEvent.RegisterEvent(panel.GetType().Name, msgId, msgFunc);

            Logger.UI?.LogError($"OnAfterShow-->{panel.GetType().Name}::{msgId}");
        }

        public override void OnHide(InternalUIPanel panel)
        {
            base.OnHide(panel);
            NetworkEvent.RemoveEvent(panel.GetType().Name);
        }
    }
}
