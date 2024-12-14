using System;
using System.Collections.Generic;

namespace IAUI
{
    /// <summary>
    /// 绑定用户数据改变事件
    /// </summary>
    public class UIUserDataChangeGlue : UIGlue
    {
        private Dictionary<InternalUserData, List<Action>> bindActionMap =
            new Dictionary<InternalUserData, List<Action>>();

        public UIUserDataChangeGlue()
        {
            
        }

        public UIUserDataChangeGlue(Dictionary<InternalUserData, List<Action>> pBindActionMap)
        {
            foreach (InternalUserData userData in pBindActionMap.Keys)
            {
                List<Action> actions = bindActionMap[userData];
                for (int i = 0; i < actions.Count; i++)
                {
                    Register(userData, actions[i]);
                }
            }
        }

        public void Register(InternalUserData pUserData, Action pChangeCallBack)
        {
            if (!bindActionMap.ContainsKey(pUserData))
            {
                bindActionMap.Add(pUserData,new List<Action>());
            }
            bindActionMap[pUserData].Add(pChangeCallBack);
            pUserData.RegUserDataChange(pChangeCallBack);
        }

        public void Remove(InternalUserData pUserData, Action pChangeCallBack)
        {
            if (!bindActionMap.ContainsKey(pUserData))
            {
                return;
            }

            List<Action> actions = bindActionMap[pUserData];
            for (int i = actions.Count-1; i >= 0; i--)
            {
                if (actions[i].Equals(pChangeCallBack))
                {
                    actions.RemoveAt(i);
                    pUserData.RemoveUserDataChange(pChangeCallBack);
                }
            }
        }

        public override void OnHide(InternalUIPanel panel)
        {
            foreach (InternalUserData userData in bindActionMap.Keys)
            {
                List<Action> actions = bindActionMap[userData];
                for (int i = 0; i < actions.Count; i++)
                {
                    userData.RemoveUserDataChange(actions[i]);
                }
            }
            bindActionMap.Clear();
        }
    }
}